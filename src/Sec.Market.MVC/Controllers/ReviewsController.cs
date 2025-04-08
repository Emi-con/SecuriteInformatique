using Ganss.Xss;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sec.Market.MVC.Interfaces;
using Sec.Market.MVC.Models;

namespace Sec.Market.MVC.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ICustomerReviewService _customerReviewService;
        private readonly IDataProtector _protector;
       
        public ReviewsController(ICustomerReviewService customerReviewService, IDataProtectionProvider provider)
        {
            _customerReviewService = customerReviewService;
            _protector = provider.CreateProtector("CustomerReviewProtection");
        }
        
        // GET: ReviewsController
        public async Task<ActionResult> Index(int id)
        {
            ViewBag.Id = id;
            var reviews = await _customerReviewService.ObtenirSelonProduit(id);

            foreach (var review in reviews)
            {  
                // Déchiffrer les infos sensibles de la BD
                review.CustomerName = _protector.Unprotect(review.CustomerName);
                review.CustomerEmail = _protector.Unprotect(review.CustomerEmail);
            }

            return View(reviews);
        }

        // GET: ReviewsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReviewsController/Create
        public ActionResult Create(int id)
        {

            return View(new CustomerReview {ProductId = id});
        }

        // POST: ReviewsController/Create
        [HttpPost]
        public async Task<ActionResult> Create(CustomerReview customerReview)
        {
            // Ajout sanitizer via package Ganss.Xss
            var sanitizer = new HtmlSanitizer();

            var commentaireOriginal = customerReview.Comment;
            customerReview.Comment = sanitizer.Sanitize(customerReview.Comment);

            // Vérification si le champ est vide suite à l'ajout du sanitizer
            if (string.IsNullOrWhiteSpace(customerReview.Comment)) 
            {
                ModelState.AddModelError("Comment", "Le commentaire est invalide.");

                return View(customerReview);
            }

            if (ModelState.IsValid)
            {    
                // Chiffrement des données sensibles
                customerReview.CustomerName = _protector.Protect(customerReview.CustomerName);
                customerReview.CustomerEmail = _protector.Protect(customerReview.CustomerEmail);

                await _customerReviewService.Ajouter(customerReview);
                return RedirectToAction(nameof(Index), new {id = customerReview.Id});
            }
            
           return View(customerReview);            
        }

        // GET: ReviewsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReviewsController/Edit/5
        [HttpPost]
       
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReviewsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReviewsController/Delete/5
        [HttpPost]
        
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
