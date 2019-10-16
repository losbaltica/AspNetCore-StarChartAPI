using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites =
                _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestialObject.OrbitedObjectId).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Name == name).ToList();

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects
                    .Where(x => x.OrbitedObjectId == celestialObject.OrbitedObjectId).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects
                    .Where(x => x.OrbitedObjectId == celestialObject.OrbitedObjectId).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);

            _context.SaveChanges();

            return CreatedAtRoute("GetById", 
                new {id = celestialObject.Id}, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var updateObject = _context.CelestialObjects.Find(id);

            if (updateObject == null)
            {
                return NotFound();
            }

            updateObject.Name = celestialObject.Name;
            updateObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            updateObject.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.Update(updateObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var renameObject = _context.CelestialObjects.Find(id);

            if (renameObject == null)
            {
                return NotFound();
            }

            renameObject.Name = name;
            _context.Update(renameObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleteObjects = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id);

            if (!deleteObjects.Any())
            {
                return NotFound();
            }

            _context.RemoveRange(deleteObjects);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
