using Ganss.Xss;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sec.Market.API.Entites;
using Sec.Market.API.Interfaces;

namespace Sec.Market.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerReviewsController : ControllerBase
    {
        private readonly ICustomerReviewRepository _customerReviewRepository; 
        
        public CustomerReviewsController(ICustomerReviewRepository customerReviewRepository)
        {
            _customerReviewRepository = customerReviewRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerReview>>> GetCustomerReviews(int productId)
        {
            return await _customerReviewRepository.GetCustomerReviewsByProduct(productId);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerReview>> GetCustomerReview(int id)
        {


            var customerReview = await _customerReviewRepository.GetCustomerReviewById(id);

            if (customerReview == null)
            {
                return NotFound();
            }

            return customerReview;
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CustomerReview>> PostCustomerReview(CustomerReview customerReview)
        {
            // Initialisation du sanitizer
            var sanitizer = new HtmlSanitizer();

            // Sanitize le commentaire
            customerReview.Comment = sanitizer.Sanitize(customerReview.Comment);

            // Vérification si le champ est vide suite à l'ajout du sanitizer
            if (string.IsNullOrWhiteSpace(customerReview.Comment))
            {
                return BadRequest("Le commentaire est invalide.");
            }

            customerReview.Id = 0;

          await _customerReviewRepository.InsertCustomerReview(customerReview);
          return CreatedAtAction("GetCustomerReview", new { id = customerReview.Id }, customerReview);           
        }
    }

}
