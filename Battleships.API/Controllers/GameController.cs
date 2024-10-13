using Battleships.Core.DTOs;
using Battleships.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace BattleshipApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GameController> _logger;

        public GameController(IGameService gameService, ILogger<GameController> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        /// <summary>
        /// Initializes the game with a new board and randomly places ships.
        /// </summary>
        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeGame(int userId)
        {
            _logger.LogInformation("Initializing game for user {UserId}.", userId);
            try
            {
                await _gameService.InitializeGameAsync(userId);
                _logger.LogInformation("Game initialized successfully for user {UserId}.", userId);
                return Ok("Game initialized with ships placed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the game for user {UserId}.", userId);
                return StatusCode(500, $"An error occurred while initializing the game: {ex.Message}");
            }
        }

        /// <summary>
        /// Fires a shot at the specified row and column.
        /// </summary>
        /// <param name="request">The shooting request containing the row and column.</param>
        [HttpPost("shoot")]
        public async Task<IActionResult> Shoot([FromBody] ShootRequestDto request)
        {
            _logger.LogInformation("User {UserId} is attempting to shoot at row {Row}, column {Column}.", request.UserId, request.Row, request.Column);
            try
            {
                if (!await _gameService.IsGameInitiatedAsync(request.UserId))
                {
                    _logger.LogWarning("User {UserId} attempted to shoot before initializing the game.", request.UserId);
                    return BadRequest("Game not initiated. Please initialize the game before shooting.");
                }

                var response = await _gameService.ShootAsync(request.UserId, request.Row, request.Column);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input for shooting by user {UserId}.", request.UserId);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation during shooting by user {UserId}.", request.UserId);
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the shot for user {UserId}.", request.UserId);
                return StatusCode(500, $"An error occurred while processing the shot: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current state of the game board.
        /// </summary>
        [HttpGet("board")]
        public async Task<IActionResult> GetBoard([FromQuery] int userId, [FromQuery] bool showPlacedShips = false)
        {
            try
            {
                if (!await _gameService.IsGameInitiatedAsync(userId))
                {
                    _logger.LogWarning("User {UserId} attempted to retrieve the board state before initializing the game.", userId);
                    return BadRequest("Game not initiated. Please initialize the game before retrieving the board.");
                }

                var boardState = await _gameService.GetBoardStateWithShipsAsync(userId, showPlacedShips);
                return Ok(boardState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the board state for user {UserId}.", userId);
                return StatusCode(500, $"An error occurred while retrieving the board state: {ex.Message}");
            }
        }

        /// <summary>
        /// Resets the game board to its initial state.
        /// </summary>
        [HttpPost("reset")]
        public async Task<IActionResult> ResetGame([FromQuery] int userId)
        {
            try
            {
                if (!await _gameService.IsGameInitiatedAsync(userId))
                {
                    _logger.LogWarning("User {UserId} attempted to reset the game before initializing it.", userId);
                    return BadRequest("Game not initiated. Please initialize the game before resetting.");
                }

                await _gameService.ResetGameAsync(userId);
                return Ok("The game has been reset.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while resetting the game for user {UserId}.", userId);
                return StatusCode(500, $"An error occurred while resetting the game: {ex.Message}");
            }
        }

        /// <summary>
        /// Exits the game.
        /// </summary>
        [HttpPost("quit")]
        public async Task<IActionResult> QuitGame([FromQuery] int userId)
        {
            try
            {
                if (!await _gameService.IsGameInitiatedAsync(userId))
                {
                    _logger.LogWarning("User {UserId} attempted to quit the game before initializing it.", userId);
                    return BadRequest("Game not initiated. Please initialize the game before quitting.");
                }

                await _gameService.QuitGameAsync(userId);
                return Ok("You exited from the game.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while quitting the game for user {UserId}.", userId);
                return StatusCode(500, $"An error occurred while quitting the game: {ex.Message}");
            }
        }
    }
}
