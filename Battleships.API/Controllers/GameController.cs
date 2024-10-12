using Battleships.Core.DTOs;
using Battleships.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace BattleshipApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        /// <summary>
        /// Initializes the game with a new board and randomly places ships.
        /// </summary>
        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeGame()
        {
            try
            {
                await _gameService.InitializeGameAsync();
                return Ok("Game initialized with ships placed.");
            }
            catch (Exception ex) // Catch-all for unexpected errors
            {
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
            try
            {
                if (!await _gameService.IsGameInitiatedAsync())
                {
                    return BadRequest("Game not initiated. Please initialize the game before shooting.");
                }

                var response = await _gameService.ShootAsync(request.Row, request.Column);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex) // Catch-all for unexpected errors
            {
                return StatusCode(500, $"An error occurred while processing the shot: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current state of the game board.
        /// </summary>
        [HttpGet("board")]
        public async Task<IActionResult> GetBoard()
        {
            try
            {
                if (!await _gameService.IsGameInitiatedAsync())
                {
                    return BadRequest("Game not initiated. Please initialize the game before retrieving the board.");
                }

                var boardState = await _gameService.GetBoardStateAsync();
                return Ok(boardState);
            }
            catch (Exception ex) // Catch-all for unexpected errors
            {
                return StatusCode(500, $"An error occurred while retrieving the board state: {ex.Message}");
            }
        }

        /// <summary>
        /// Resets the game board to its initial state.
        /// </summary>
        [HttpPost("reset")]
        public async Task<IActionResult> ResetGame()
        {
            try
            {
                if (!await _gameService.IsGameInitiatedAsync())
                {
                    return BadRequest("Game not initiated. Please initialize the game before resetting.");
                }

                await _gameService.ResetGameAsync();
                return Ok("The game has been reset.");
            }
            catch (Exception ex) // Catch-all for unexpected errors
            {
                return StatusCode(500, $"An error occurred while resetting the game: {ex.Message}");
            }
        }
    }
}
