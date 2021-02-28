using System.Security.Cryptography.X509Certificates;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JeffSite.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JeffSite.Controllers
{
    public class MallingController : Controller
    {
        private readonly SocialMidiaService _socialMidia;
        private readonly MallingService _mallingService;
        private readonly ConfiguracaoService _configuracaoService;

        public MallingController(MallingService mallingService, SocialMidiaService socialMidia, ConfiguracaoService configuracaoService)
        {
            _mallingService = mallingService;
            _socialMidia = socialMidia;
            _configuracaoService = configuracaoService;
        }

        [HttpGet]
        public IActionResult Index(){
            var userLogged = HttpContext.Session.GetString("userLogged");
            if (userLogged == "" || userLogged == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            var itens = _mallingService.FillAllMalling();
            ViewData["Title"] = "Lista de email";
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
            return View();
        }

        [HttpGet]
        [Route("remover-email/{email}")]
        public IActionResult RemoverEmail(string email){
            ViewBag.Redes = _socialMidia.FindAll();
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("remover-email")]
        public IActionResult RemoverEmail(bool opcao, string email){
            ViewBag.Redes = _socialMidia.FindAll();
            
            if(opcao){
                _mallingService.DeleteEmail(email);
                ViewBag.Message = "Email removido!";
            }else{
                ViewBag.Message = "Email mantido!";
            }

            return RedirectToAction("RemoverEmail",email);
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
            var config = _configuracaoService.FindEmail();
            var emailFrom = _configuracaoService.FindAdminEmail();
            var flag = JeffSite.Utils.EnviarEmail.enviarEmailMalling(config,emailFrom,emails,titulo,html);

            if(flag){
                ViewBag.Message = "sucesso";
            }else{
                ViewBag.Message = "erro";
            }
            return View();
        }
    }
}