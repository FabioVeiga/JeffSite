using System.Security.Cryptography.X509Certificates;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JeffSite.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JeffSite.Models;

namespace JeffSite.Controllers
{
    public class MallingController : Controller
    {
        private readonly SocialMidiaService _socialMidia;
        private readonly MallingService _mallingService;
        private readonly ConfiguracaoService _configuracaoService;
        private readonly LeitorService _leitorService;

        public MallingController(MallingService mallingService, SocialMidiaService socialMidia, ConfiguracaoService configuracaoService, LeitorService leitorService)
        {
            _mallingService = mallingService;
            _socialMidia = socialMidia;
            _configuracaoService = configuracaoService;
            _leitorService = leitorService;
        }

        [HttpGet]
        public IActionResult Index(string filtro, int limit = 10){
            var userLogged = HttpContext.Session.GetString("userLogged");
            if (userLogged == "" || userLogged == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            List<Malling> itens = new List<Malling>();
            if(!string.IsNullOrEmpty(filtro)){
                itens = _mallingService.FillAllMallingWithFilters(limit, filtro);
            }else{
                itens = _mallingService.FillAllMalling(limit);
            }
            
            ViewBag.QuantidadeDeAprovacao = _leitorService.HowManyPostsAreNotApproved();
            ViewData["Title"] = "Lista de email";
            ViewBag.Limit = limit;
            ViewBag.Filtro = filtro;
            return View(itens);
        }

        [HttpGet]
        public IActionResult EnviarEmailMailling(){
            var userLogged = HttpContext.Session.GetString("userLogged");
            if (userLogged == "" || userLogged == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            ViewData["Title"] = "Enviar email mailling";
            ViewBag.QuantidadeDeAprovacao = _leitorService.HowManyPostsAreNotApproved();
            return View();
        }

        [HttpGet]
        [Route("remover-email/{email}")]
        public IActionResult RemoverEmail(bool opcao, string email){
            ViewBag.Redes = _socialMidia.FindAll();
            _mallingService.DeleteEmail(email);
            return View("RemoverEmail");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnviarEmailMailling(string titulo, string html){
            ViewData["Title"] = "Enviar email mailling";
            if(string.IsNullOrEmpty(titulo) || string.IsNullOrEmpty(html)){
                ViewBag.Obrigatorio = "Campo obrigatorio!";
                return View("EnviarEmailMailling", titulo);
            }

            var emails = _mallingService.FillAllMallingJusEmail();
            var configEmail = _configuracaoService.FindEmail();
            var configSite = _configuracaoService.Find();
            var emailFrom = _configuracaoService.FindAdminEmail();
            List<Dictionary<bool,string>> flags = new List<Dictionary<bool,string>>();
            foreach (var email in emails)
            {
                Dictionary<bool, string> item = new Dictionary<bool, string>(); 
                item.Add(JeffSite.Utils.EnviarEmail.enviarEmailMalling(configEmail,emailFrom,email,titulo,html, configSite.UrlSite),email);
                flags.Add(item);
            }
            ViewBag.Itens = flags;
            return View();
        }
    }
}