using ProductApi.Models;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;

namespace ProductApi.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private static ConcurrentDictionary<string, Product> _products = new ConcurrentDictionary<string, Product>();

        [Route("{id}", Name = "GetById")]
        public IHttpActionResult Get(string id)
        {
            Product product = null;
            if (_products.TryGetValue(id, out product))
            {
                return Ok(product);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post(Product product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            product.Id = Guid.NewGuid().ToString();
            _products[product.Id] = product;
            return CreatedAtRoute("GetById", new { id = product.Id }, product);
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(string id, Product product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (product.Id != id)
            {
                return BadRequest("product.id does not match id parameter");
            }
            
            if (!_products.Keys.Contains(id))
            {
                return NotFound();
            }

            _products[id] = product;
            return new StatusCodeResult(HttpStatusCode.NoContent, this);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            Product product = null;
            _products.TryRemove(id, out product);
            return new StatusCodeResult(HttpStatusCode.NoContent, this);
        }
    }
}
