﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jupiterCore.jupiterContext;
using Jupiter.ActionFilter;
using Jupiter.Controllers;
using Jupiter.Models;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public CartsController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Carts
        [HttpGet]
        public ActionResult<List<Cart>> GetCart()
        {
            var cartsValue = _context.Cart.Include(s => s.Contact).Include(s => s.CartProd).ToList();
            return Ok(cartsValue);
        }

        // GET: api/Carts/5
        [HttpGet("{id}")]
        public ActionResult GetCart(int id)
        {
            var cart1 =  _context.Cart.Include(s => s.Contact).Include(s => s.CartProd)
                .FirstOrDefault(s => s.CartId == id);
            return Ok(cart1);
        }

        // PUT: api/Carts/5
        [CheckModelFilter]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutCart(int id,  CartModel cartModel)
        {
            var result = new Result<string>();
            Type cartType = typeof(Cart);
            var updateCart = await _context.Cart.Where(x=>x.CartId == id).FirstOrDefaultAsync();
            if (updateCart == null)
            {
                return NotFound(DataNotFound(result));
            }
            UpdateTable(cartModel,cartType,updateCart);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsSuccess = false;
                return BadRequest(result);
            }
            return Ok(result);
        }

        // POST: api/Carts
        [CheckModelFilter]
        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart(CartModel cartModel)
        {
            var result = new Result<Cart>();
            Cart cart = new Cart();
            _mapper.Map(cartModel, cart);
            try
            {
                result.Data = cart;
                await _context.Cart.AddAsync(cart);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
            }
            return Ok(result);
        }

        // DELETE: api/Carts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Cart>> DeleteCart(int id)
        {
            var result = new Result<string>();
            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound(DataNotFound(result));
            }
            cart.IsActivate = 0;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsSuccess = false;
            }
            return Ok(result);
        }
    }
}