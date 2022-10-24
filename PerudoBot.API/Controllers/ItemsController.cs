using Microsoft.AspNetCore.Mvc;
using PerudoBot.API.Constants;
using PerudoBot.API.DTOs;
using PerudoBot.API.Filters;
using PerudoBot.API.Services;
using PerudoBot.Database.Data;

namespace PerudoBot.API.Controllers
{
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly PerudoBotDbContext _db;

        public ItemsController(PerudoBotDbContext context)
        {
            _db = context;
        }

        [HttpGet]
        [Route("items")]
        public IResult GetItems()
        {
            var items = _db.Items.ToList();
            return Results.Ok(new { data = items });
        }


        [HttpGet]
        [Route("items/{itemId}")]
        public IResult GetItem(int itemId)
        {
            var item = _db.Items.SingleOrDefault(x => x.Id == itemId);

            if (item == null)
            {
                return Results.BadRequest(new { error = "Item not found" });
            }

            return Results.Ok(new { data = item });
        }

        [HttpDelete]
        [Route("item/{itemId}")]
        public IResult DeleteItem(int itemId)
        {
            var item = _db.Items.SingleOrDefault(x => x.Id == itemId);

            if (item == null) 
            {
                return Results.BadRequest(new { error = "Item not found" });
            }

            _db.Items.Remove(item);
            _db.SaveChanges();

            return Results.Ok();
        }

        [HttpPost]
        [Route("items/add")]
        public IResult AddItem(Item item)
        {
            _db.Items.Add(item);
            _db.SaveChanges();
            return Results.Ok();
        }

        [HttpPost]
        [Route("items/update")]
        public IResult UpdateItem(Item item)
        {
            _db.Items.Update(item);
            _db.SaveChanges();
            return Results.Ok();
        }
    }
}
