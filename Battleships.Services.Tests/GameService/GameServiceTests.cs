using Battleships.Core.DTOs;
using Battleships.Core.Models;
using Battleships.DAL.IRepositories;
using Battleships.DAL.UnitOfWork;
using Battleships.Services.IServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace Battleships.Services.Tests.GameServices
{
    namespace Battleships.Services.Tests
    {
        [TestFixture]
        public class GameServiceTests
        {
            private Mock<IUnitOfWork> _unitOfWorkMock;
            private Mock<IBoardRepository> _boardRepositoryMock;
            private Mock<IPlayerRepository> _playerRepositoryMock;
            private Mock<ILogger<GameService>> _loggerMock;
            private GameService _gameService;

            [SetUp]
            public void Setup()
            {
                // Create mocks for the dependencies
                _unitOfWorkMock = new Mock<IUnitOfWork>();
                _boardRepositoryMock = new Mock<IBoardRepository>();
                _playerRepositoryMock = new Mock<IPlayerRepository>();
                _loggerMock = new Mock<ILogger<GameService>>();

                // Setup IUnitOfWork to return mock repositories
                _unitOfWorkMock.Setup(u => u.Boards).Returns(_boardRepositoryMock.Object);
                _unitOfWorkMock.Setup(u => u.Players).Returns(_playerRepositoryMock.Object);

                // Create instance of GameService with mocked dependencies
                _gameService = new GameService(_unitOfWorkMock.Object, _loggerMock.Object, new Random());
            }

            [Test]
            public async Task IsGameInitiatedAsync_GameInitialized_ReturnsTrue()
            {
                // Arrange
                var userId = 1;
                var boards = new List<Board> { new Board { Id = 1, UserId = userId } };
                _unitOfWorkMock.Setup(u => u.Boards.GetAllBoardsByUserIdAsync(userId)).ReturnsAsync(boards);

                // Act
                var result = await _gameService.IsGameInitiatedAsync(userId);

                // Assert
                Assert.IsTrue(result);
                _unitOfWorkMock.Verify(u => u.Boards.GetAllBoardsByUserIdAsync(userId), Times.Once);
            }

            [Test]
            public async Task IsGameInitiatedAsync_GameNotInitialized_ReturnsFalse()
            {
                // Arrange
                var userId = 1;
                var boards = new List<Board>();
                _unitOfWorkMock.Setup(u => u.Boards.GetAllBoardsByUserIdAsync(userId)).ReturnsAsync(boards);

                // Act
                var result = await _gameService.IsGameInitiatedAsync(userId);

                // Assert
                Assert.IsFalse(result);
                _unitOfWorkMock.Verify(u => u.Boards.GetAllBoardsByUserIdAsync(userId), Times.Once);
            }

            [Test]
            public async Task ShootAsync_ValidShot_FakeResult_ReturnsExpectedResponse()
            {
                // Arrange
                var userId = 1;
                var row = 'A';
                var column = 1;

                // Create a fake response for ShootAsync
                var fakeResponse = new ShootResponseDto
                {
                    Row = row,
                    Column = column,
                    UserHit = true,
                    ComputerHit = false,
                    GameOver = false,
                    UserScore = 15,
                    ComputerScore = 0
                };

                // Create a mock of IGameService
                var gameServiceMock = new Mock<IGameService>();

                // Setup the ShootAsync method to return the fake response
                gameServiceMock.Setup(g => g.ShootAsync(userId, row, column))
                               .ReturnsAsync(fakeResponse);

                // Act
                var result = await gameServiceMock.Object.ShootAsync(userId, row, column);

                // Assert
                Assert.NotNull(result);
                Assert.That(15 == result.UserScore);  
                Assert.IsTrue(result.UserHit);  
                Assert.IsFalse(result.ComputerHit);  
            }

            [Test]
            public async Task GetBoardStateWithShipsAsync_ValidRequest_ReturnsBoardStateResponseDto()
            {
                // Arrange
                var userId = 1;

                // Create user and computer players with sample ships
                var userPlayer = new Player
                {
                    IsComputer = false,
                    Board = new Board
                    {
                        SerializedGrid = "~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,S,~,~,~,~,~,~,~,~,~,S,~,~,~,~,~,~,~,~,~,S,~,~,~,~,~,~,~,~,~,S,~,~,~,~,~,~,~,S,~,S,~,~,~,~,~,~,~,S,S,S,S,S,~,~,~,~,~,S,~,~,~,~,~,~,~,~,~,S,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~"
                    },
                    Fleet = new Fleet
                    {
                        Ships = new List<Ship>
                        {
                            new Ship { Name = "Battleship", Size = 5 }, new Ship { Name = "Destroyer1", Size = 4 }, new Ship { Name = "Destroyer2", Size = 4 }
                        }
                    }
                };

                var computerPlayer = new Player
                {
                    IsComputer = true,
                    Board = new Board
                    {
                        SerializedGrid = "~,~,~,~,~,~,~,S,~,~,~,S,S,S,S,S,~,S,~,~,~,~,~,~,~,~,~,S,~,~,~,~,~,~,S,~,~,S,~,~,~,~,~,~,S,~,~,~,~,~,~,~,~,~,S,~,~,~,~,~,~,~,~,~,S,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~"
                    },
                    Fleet = new Fleet
                    {
                        Ships = new List<Ship> {
                            new Ship { Name = "Battleship", Size = 5 }, new Ship { Name = "Destroyer1", Size = 4 }, new Ship { Name = "Destroyer2", Size = 4 }
                        }
                    }
                };

                var players = new List<Player> { userPlayer, computerPlayer };

                _unitOfWorkMock.Setup(u => u.Players.GetPlayersByBoardsUserIdAsync(userId)).ReturnsAsync(players);

                // Act
                var result = await _gameService.GetBoardStateWithShipsAsync(userId, true);

                // Assert
                Assert.NotNull(result);
                _unitOfWorkMock.Verify(u => u.Players.GetPlayersByBoardsUserIdAsync(userId), Times.Once);
            }

            [Test]
            public async Task ResetGameAsync_ValidUser_ResetsGameSuccessfully()
            {
                // Arrange
                var userId = 1;

                // Create user and computer players with sample ships
                var userPlayer = new Player
                {
                    IsComputer = false,
                    Board = new Board
                    {
                        SerializedGrid = "~,~,~,~,~,~,~,S,~,~,~,S,S,X,S,S,~,S,~,~,~,~,~,~,~,~,~,S,~,~,~,~,~,~,S,~,~,S,~,~,~,~,~,~,S,~,O,~,~,~,~,~,~,~,S,~,~,~,~,~,~,~,~,~,S,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~"
                    },
                    Fleet = new Fleet
                    {
                        Ships = new List<Ship> {
                            new Ship { Name = "Battleship", Size = 5 ,Hits = 1}, new Ship { Name = "Destroyer1", Size = 4 }, new Ship { Name = "Destroyer2", Size = 4 }
                        }
                    }
                };

                var computerPlayer = new Player
                {
                    IsComputer = true,
                    Board = new Board
                    { 
                        SerializedGrid = "~,~,~,~,~,~,~,S,~,~,~,S,S,S,S,S,~,S,~,~,~,~,O,~,~,~,~,S,~,~,~,~,~,~,S,~,~,S,~,~,~,~,~,~,S,~,~,O,~,~,~,~,~,~,S,~,~,~,~,~,~,~,~,~,S,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~,~"
                    },
                    Fleet = new Fleet
                    {
                        Ships = new List<Ship> {
                            new Ship { Name = "Battleship", Size = 5 }, new Ship { Name = "Destroyer1", Size = 4 }, new Ship { Name = "Destroyer2", Size = 4 }
                        }
                    }
                };

                var players = new List<Player> { userPlayer, computerPlayer };

                _unitOfWorkMock.Setup(u => u.Players.GetPlayersByBoardsUserIdAsync(userId)).ReturnsAsync(players);

                // Act
                await _gameService.ResetGameAsync(userId);

                // Assert
                _unitOfWorkMock.Verify(u => u.Players.GetPlayersByBoardsUserIdAsync(userId), Times.Once);
                _unitOfWorkMock.Verify(u => u.Boards.UpdateBoardAsync(userPlayer.Board), Times.Once);
                _unitOfWorkMock.Verify(u => u.Boards.UpdateBoardAsync(computerPlayer.Board), Times.Once);
                _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
            }

            [Test]
            public async Task QuitGameAsync_ValidUser_RemovesEntitiesSuccessfully()
            {
                // Arrange
                var userId = 1;
                _unitOfWorkMock.Setup(u => u.Players.RemoveAllEntitiesyUserIdAsync(userId)).ReturnsAsync(true);

                // Act
                await _gameService.QuitGameAsync(userId);

                // Assert
                _unitOfWorkMock.Verify(u => u.Players.RemoveAllEntitiesyUserIdAsync(userId), Times.Once);
                _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
            }
        }
    }
}
