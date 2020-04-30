using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Models;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SecretHitlerBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DummyController : ControllerBase
    {
        private readonly DummyContext _context;

        public DummyController(DummyContext context)
        {
            _context = context;
            Bag bag1 = new Bag()
            {
                Name = "Bag1",
                
            };
            _context.Bags.Add(bag1);
            _context.SaveChangesAsync();
            Dummy dummy1 = new Dummy()
            {
                Name = "d1",
                BagId = 1
            };
            Dummy dummy2 = new Dummy()
            {
                Name = "d2",
                BagId = 1
            };
            _context.Dummies.Add(dummy1);
            _context.Dummies.Add(dummy2);
            _context.SaveChangesAsync();
            
            
        }

        [HttpGet]
        public ActionResult<IEnumerable<Bag>> Get()
        {
            return _context.Bags;
        }
    }
}