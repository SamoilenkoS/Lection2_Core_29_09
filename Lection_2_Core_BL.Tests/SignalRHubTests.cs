using Lection2_Core.Core;
using Lection2_Core_API;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lection2_Core_BL.Tests
{
    public class SignalRHubTests
    {
        private Mock<IHubCallerClients<ISignalRClient>> _clientsMock;
        private Mock<HubCallerContext> _contextMock;
        private Mock<IConnectionsStorage> _connectionStorage;
        private Mock<IMessageStorage> _messageStorage;
        private SignalRHub _hub;

        [SetUp]
        public void Setup()
        {
            _clientsMock = new Mock<IHubCallerClients<ISignalRClient>>();
            _contextMock = new Mock<HubCallerContext>();
            _connectionStorage = new Mock<IConnectionsStorage>();
            _messageStorage = new Mock<IMessageStorage>();
            _hub = new SignalRHub(
               _connectionStorage.Object,
               _messageStorage.Object);
            _hub.Context = _contextMock.Object;
            _hub.Clients = _clientsMock.Object;
        }

        [Test]
        public async Task Test()
        {
            var message = "asdasd";
            var connectionId = "connection";
            var nickname = "nickname";
            _contextMock.Setup(x => x.ConnectionId)
                .Returns(connectionId)
                .Verifiable();
            _connectionStorage.Setup(x => x.GetUserNickname(It.Is<string>(x => x == connectionId)))
                .Returns(nickname)
                .Verifiable();
            _connectionStorage.Setup(x => x.GetPublicUserInfo(It.Is<string>(x => x == connectionId)))
                .Returns(new PublicUserInfo { Nickname = nickname })
                .Verifiable();
            _clientsMock.Setup(x => x.Others.GetMessage(
                It.Is<MessageSnapshot>(x =>
                    x.Message == message &&
                    x.IsPersonal == false &&
                    x.SenderUserInfo.Nickname == nickname &&
                    x.ReceiverNickname == null)))
                    .Verifiable();
            _messageStorage.Setup(x => x.Add(It.Is<MessageSnapshot>(x =>
                    x.Message == message &&
                    x.IsPersonal == false &&
                    x.SenderUserInfo.Nickname == nickname &&
                    x.ReceiverNickname == null)))
                    .Verifiable();

            await _hub.SendMessageToAll(message);

            _contextMock.Verify();
            _connectionStorage.Verify();
            _clientsMock.Verify();
            _messageStorage.Verify();
        }
    }
}
