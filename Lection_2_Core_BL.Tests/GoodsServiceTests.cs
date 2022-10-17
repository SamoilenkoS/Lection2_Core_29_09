using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Lection2_Core_BL.DTOs;
using Lection2_Core_BL.Services;
using Lection2_Core_DAL;
using Lection2_Core_DAL.DTOs;
using Lection2_Core_DAL.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lection2_Core_BL.Tests
{
    internal class GoodsServiceTests
    {
        private Mock<IGenericRepository<Good>> _genericGoodRepository;
        private Mock<IMapper> _mapperMock;
        private Fixture _fixture;
        
        [SetUp]
        public void Setup()
        {
            _genericGoodRepository = new Mock<IGenericRepository<Good>>();
            _mapperMock = new Mock<IMapper>();
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetAllAsync_WhenCalled_ShouldReturnGoodsList()
        {
            var goodsList = _fixture.CreateMany<Good>(2);
            var goodDto = _fixture.Create<GoodDto>();
            _genericGoodRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(goodsList)
                .Verifiable();

            _mapperMock
                .Setup(x => x.Map<IEnumerable<GoodDto>>(goodsList))
                .Returns(new List<GoodDto> { goodDto })
                .Verifiable();

            var goodsService = new GoodsService(
                _genericGoodRepository.Object,
                _mapperMock.Object);

            var response = await goodsService.GetAllAsync();

            response.Should().BeEquivalentTo(new List<GoodDto> { goodDto });
            _mapperMock.Verify();
            _genericGoodRepository.Verify();
        }

        [Test]
        public async Task GetById_WhenItemExist_ShouldReturnItem()
        {
            var good = _fixture.Create<Good>();
            var goodDto = _fixture.Create<GoodDto>();
            _genericGoodRepository
                .Setup(x => x.GetByIdAsync(good.Id))
                .ReturnsAsync(good)
                .Verifiable();

            _mapperMock
                .Setup(x => x.Map<GoodDto>(good))
                .Returns(goodDto)
                .Verifiable();

            var goodsService = new GoodsService(
                _genericGoodRepository.Object,
                _mapperMock.Object);

            var response = await goodsService.GetByIdAsync(good.Id);

            response.Should().BeEquivalentTo(goodDto);
            _mapperMock.Verify();
            _genericGoodRepository.Verify();
        }

        [Test]
        public async Task Create_WhenValidItem_ShouldAddItemToDb()
        {
            var createGoodDto = _fixture.Create<CreateGoodDto>();
            var goodFromMapperForRepository = _fixture.Create<Good>();
            goodFromMapperForRepository.Category = Category.Clothes;
            goodFromMapperForRepository.Price = 1000;
            var goodFromRepository = _fixture.Create<Good>();
            var expectedGoodDto = _fixture.Create<GoodDto>();
            createGoodDto.Title = "A";
            goodFromMapperForRepository.Title = "B";
            goodFromRepository.Title = "C";
            expectedGoodDto.Title = "D";
            
            _genericGoodRepository
                .Setup(x => x.CreateAsync(It.Is<Good>(x
                    =>
                        x.Price == goodFromMapperForRepository.Price &&
                        x.Category == goodFromMapperForRepository.Category &&
                        x.Title == goodFromMapperForRepository.Title &&
                        x.Id == goodFromMapperForRepository.Id &&
                        x.Description == goodFromMapperForRepository.Description)))
                .ReturnsAsync(goodFromRepository)
                .Verifiable();

            _mapperMock
                .Setup(x => x.Map<Good>(It.Is<CreateGoodDto>(x
                    =>
                        x.Price == createGoodDto.Price &&
                        x.Category == createGoodDto.Category &&
                        x.Title == createGoodDto.Title &&
                        x.Description == createGoodDto.Description)))
                .Returns(goodFromMapperForRepository)
                .Verifiable();

            _mapperMock
                .Setup(x => x.Map<GoodDto>(It.Is<Good>(x
                    =>
                        x.Id == goodFromRepository.Id &&
                        x.Price == goodFromRepository.Price &&
                        x.Title == goodFromRepository.Title &&
                        x.Category == goodFromRepository.Category &&
                        x.Description == goodFromRepository.Description)))
                .Returns(expectedGoodDto)
                .Verifiable();

            var goodsService = new GoodsService(
                _genericGoodRepository.Object,
                _mapperMock.Object);

            var actualGoodDto = await goodsService.CreateAsync(createGoodDto);

            actualGoodDto.Should().BeEquivalentTo(expectedGoodDto);
            _mapperMock.Verify();
            _genericGoodRepository.Verify();
        }
    }
}
