using AutoFixture;
using FluentAssertions;
using Lection2_Core.Core;
using Lection2_Core_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lection2_Core_BL.Tests
{
    public class MessageStorageTests
    {
        private readonly Fixture _fixture;
        
        public MessageStorageTests()
        {
            _fixture = new Fixture();
        }
        /// <summary>
        /// Given a message storage with 10 global messages
        /// When caller get 5 last messages
        /// Then he should get last 5 messages
        /// </summary>
        [Test]
        public void ShouldReturnLastGlobalMessagesWithRequestedCount()
        {
            // Arrange
            var callerNickname = "1111Nickname";
            var messageStorage = new MessageStorage();
            var expectedResult = new List<MessageSnapshot>();
            for (int i = 0; i < 10; i++)
            {
                var message = GetMessage();
                messageStorage.Add(message);
                expectedResult.Add(message);
            }
            expectedResult = expectedResult.Skip(5).Take(5).ToList();

            // Act
            var messages = messageStorage.GetRecent(callerNickname, 5);

            // Assert
            messages.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Given a message storage with 1 global and 1 personal messages repeated 5 times
        /// And personal messages are targeted to other user
        /// When caller get last 5 messages
        /// Then he should get last 5 global messages
        /// </summary>
        [Test]
        public void ShouldReturnOnlyGlobalMessages_WhenPersonalMessagesAreForOtherTarget()
        {
            // Arrange
            var callerNickname = "1111Nickname";
            var messageStorage = new MessageStorage();
            var expectedResult = new List<MessageSnapshot>();
            for (int i = 0; i < 5; i++)
            {
                var message = GetMessage();

                messageStorage.Add(message);
                expectedResult.Add(message);

                messageStorage.Add(GetMessage(isPersonal: true, receiverNickname: "2222"));
            }

            // Act
            var messages = messageStorage.GetRecent(callerNickname, 5);

            // Assert
            messages.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Given a message storage with 1 global and 1 personal messages repeated 5 times
        /// And personal messages are targeted to caller
        /// When caller get last 10 messages
        /// Then he should get last 10 messages
        /// </summary>
        [Test]
        public void ShouldReturnPersonalAndGlobalMessages_WhenPersonalMessagesAreForSender()
        {
            // Arrange
            var callerNickname = "1111Nickname";
            var messageStorage = new MessageStorage();
            List<MessageSnapshot> expectedResult = new List<MessageSnapshot>();
            for (int i = 0; i < 5; i++)
            {
                var message = GetMessage();

                messageStorage.Add(message);
                expectedResult.Add(message);

                var personalMessage = GetMessage(
                    isPersonal: true,
                    receiverNickname: callerNickname);

                messageStorage.Add(personalMessage);
                expectedResult.Add(personalMessage);
            }

            // Act
            var messages = messageStorage.GetRecent(callerNickname, 10);

            // Assert
            messages.Should().BeEquivalentTo(expectedResult);
        }
        
        /// <summary>
        /// Given a message storage with 1 global and 1 personal messages which he is sender repeated 5 times
        /// When caller get last 10 messages
        /// Then he should get last 10 messages
        /// </summary>
        [Test]
        public void ShouldReturnSenderPersonalAndGlobalMessages()
        {
            // Arrange
            var callerNickname = "1111Nickname";
            var messageStorage = new MessageStorage();
            List<MessageSnapshot> expectedResult = new List<MessageSnapshot>();
            for (int i = 0; i < 5; i++)
            {
                var message = GetMessage();

                messageStorage.Add(message);
                expectedResult.Add(message);

                var personalMessage = GetMessage(callerNickname, isPersonal: true);

                messageStorage.Add(personalMessage);
                expectedResult.Add(personalMessage);
            }

            // Act
            var messages = messageStorage.GetRecent(callerNickname, 10);

            // Assert
            messages.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void ShouldGetAllRelatedMessageExceptOtherPersonal()
        {
            // Arrange
            var callerNickname = "1111Nickname";
            var messageStorage = new MessageStorage();
            List<MessageSnapshot> expectedResult = new List<MessageSnapshot>();
            for (int i = 0; i < 5; i++)
            {
                var message = GetMessage();

                messageStorage.Add(message);
                expectedResult.Add(message);

                messageStorage.Add(GetMessage(isPersonal: true));

                var personalMessage = GetMessage(callerNickname, isPersonal: true);

                messageStorage.Add(personalMessage);
                expectedResult.Add(personalMessage);

                messageStorage.Add(GetMessage(isPersonal: true));

                personalMessage = GetMessage(
                    receiverNickname: callerNickname,
                    isPersonal: true);

                messageStorage.Add(personalMessage);
                expectedResult.Add(personalMessage);

                messageStorage.Add(GetMessage(isPersonal: true));
            }

            // Act
            var messages = messageStorage.GetRecent(callerNickname, 15);

            // Assert
            messages.Should().BeEquivalentTo(expectedResult);
        }

        private MessageSnapshot GetMessage(
            string? senderNickname = null,
            string? receiverNickname = null,
            bool isPersonal = false
            )
        {
            return new MessageSnapshot
            {
                IsPersonal = isPersonal,
                Message = _fixture.Create<string>(),
                ReceiverNickname = receiverNickname ?? _fixture.Create<string>(),
                SenderUserInfo = new PublicUserInfo { Nickname = senderNickname } ?? _fixture.Create<PublicUserInfo>()
            };
        }
    }
}
