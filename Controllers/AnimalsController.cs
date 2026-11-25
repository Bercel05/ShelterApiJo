using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShelterApiJo.Models;
using ShelterApiJo.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ShelterApiJo.Controllers
{
    [Route("animals")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly AnimalshelterContext _context;

        public AnimalsController(AnimalshelterContext context)
        {
            _context = context;
        }

        [HttpPost("newAnimal")]
        public async Task<IActionResult> NewAnimal([FromBody] NewAnimalDto dto)
        {
            try
            {
                if (!Guid.TryParse(dto.ShelterId, out Guid shelterGuid))
                {
                    return NotFound(new { message = "A megadott menhely nem található.", result = (object?)null });
                }

                bool shelterExists = await _context.Shelters.AnyAsync(s => s.Id == shelterGuid);
                if (!shelterExists)
                {
                    return NotFound(new { message = "A megadott menhely nem található.", result = (object?)null });
                }
                var newAnimal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Species = dto.Species,
                    Age = dto.age,
                    Weight = dto.weight,
                    RegTime = DateTime.Now,
                    ShelterId = shelterGuid
                };

                _context.Animals.Add(newAnimal);
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created, new
                {
                    message = "Az állat sikeresen rögzítve.",
                    result = newAnimal
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = $"Hiba történt az állat rögzítésekor: {ex.Message}",
                    result = (object?)null
                });
            }
        }

     
        [HttpGet("allAnimal")]
        public async Task<IActionResult> AllAnimal()
        {
            try
            {
                var animals = await _context.Animals.ToListAsync();
                if (animals.Count == 0)
                {
                    return NotFound(new { message = "Nincs egyetlen állat sem rögzítve.", result = (object?)null });
                }

                return Ok(new
                {
                    message = "Állatok sikeresen lekérdezve.",
                    result = animals
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = $"Hiba történt az állatok listázásakor: {ex.Message}",
                    result = (object?)null
                });
            }
        }

     
        [HttpGet("byId")]
        public async Task<IActionResult> ById([FromQuery] Guid id)
        {
            try
            {
                var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
                if (animal == null)
                {
                    return NotFound(new { message = "A megadott azonosítóval nem található állat.", result = (object?)null });
                }

                return Ok(new
                {
                    message = "Állat sikeresen lekérdezve.",
                    result = animal
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = $"Hiba történt az állat lekérdezésekor: {ex.Message}",
                    result = (object?)null
                });
            }
        }

      
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromQuery] Guid id, [FromBody] UpdateAnimalDto dto)
        {
            try
            {
                var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
                if (animal == null)
                {
                    return NotFound(new { message = "A módosítandó állat nem található.", result = (object?)null });
                }

                bool shelterExists = await _context.Shelters.AnyAsync(s => s.Id == dto.ShelterId);
                if (!shelterExists)
                {
                    return NotFound(new { message = "A megadott új menhely nem található.", result = (object?)null });
                }

                animal.Name = dto.Name;
                animal.Species = dto.Species;
                animal.Age = dto.age;
                animal.Weight = dto.weight;
                animal.ShelterId = dto.ShelterId;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Az állat adatai sikeresen módosítva.",
                    result = animal
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = $"Hiba történt az állat módosításakor: {ex.Message}",
                    result = (object?)null
                });
            }
        }

       
        [HttpDelete("deleteAnimal")]
        public async Task<IActionResult> DeleteAnimal([FromQuery] Guid id)
        {
            try
            {
                var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
                if (animal == null)
                {
                    return NotFound(new { message = "A törlendő állat nem található.", result = (object?)null });
                }

                _context.Animals.Remove(animal);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Az állat sikeresen törölve.",
                    result = animal
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = $"Hiba történt az állat törlésekor: {ex.Message}",
                    result = (object?)null
                });
            }
        }
    }
}