using Battleships.Core.DTOs;
using Battleships.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace BattleshipApi.Controllers
{
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
                await _gameService.InitializeGameAsync();
                return Ok("Game initialized with ships placed.");
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
            }

            /// <summary>
            /// Gets the current state of the game board.
            /// </summary>
            [HttpGet("board")]
            public async Task<IActionResult> GetBoard()
            {
                var boardState = await _gameService.GetBoardStateAsync();
                return Ok(boardState);
            }

            /// <summary>
            /// Resets the game board to its initial state.
            /// </summary>
            [HttpPost("reset")]
            public async Task<IActionResult> ResetGame()
            {
                await _gameService.ResetGameAsync();
                return Ok("The game has been reset.");
            }
        }
    }
}