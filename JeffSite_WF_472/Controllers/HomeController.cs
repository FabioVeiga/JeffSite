﻿using JeffSite_WF_472.Data;
using JeffSite_WF_472.Models;
using JeffSite_WF_472.Services;
using JeffSite_WF_472.Utils;

using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace JeffSite_WF_472.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        private readonly ConfiguracaoService _configuracaoService;
        private readonly SocialMidiaService _socialMidia;
        private readonly CarouselService _carouselService;
        private readonly MallingService _mallingService;
        private readonly LeitorService _leitorService;
        private readonly JeffContext _jeffContext;

        public HomeController(JeffContext jeffContext, UserService userService, ConfiguracaoService configuracaoService, SocialMidiaService socialMidia, CarouselService carouselService, MallingService mallingService, LeitorService leitorService)
        {
            _jeffContext = jeffContext;
            _userService = userService;
            _configuracaoService = configuracaoService;
            _socialMidia = socialMidia;
            _carouselService = carouselService;
            _mallingService = mallingService;
            _leitorService = leitorService;

        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Redes = _socialMidia.FindAll();
            ViewBag.Carousels = _carouselService.FindAllActive();
            ViewBag.CarouselsQuantity = _carouselService.Quantity();
            return View();
        }

        public ActionResult BioCompleta()
        {
            ViewBag.Redes = _socialMidia.FindAll();
            ViewBag.Biogragia = _configuracaoService.Find().Biography;
            if (ViewBag.Biogragia == "<p><br></p>")
                return View("BioCompleta_OLD");

            return View();
        }

        public ActionResult Contato()
        {
            ViewBag.Redes = _socialMidia.FindAll();
            return View();
        }

        [HttpPost]
        //[AutoValidateAntiforgeryToken]
        public ActionResult Contato(int numA, int numB, string ope, int resposta, string namecontact, string emailcontact, string phonecontact, string subjectcontact)
        {
            ViewBag.Redes = _socialMidia.FindAll();
            if (!string.IsNullOrEmpty(ope))
            {
                int resp = 0;
                switch (ope)
                {
                    case "+":
                        resp = numA + numB;
                        break;
                    case "-":
                        resp = numA - numB;
                        break;
                    default:
                        resp = numA * numB;
                        break;
                }
                if (resp != resposta)
                {
                    ViewBag.conta = "Resultado errado";
                    ViewBag.NameContact = namecontact;
                    ViewBag.EmailContact = emailcontact;
                    ViewBag.PhoneContact = phonecontact;
                    ViewBag.SubjectContact = subjectcontact;
                    return View();
                }
            }

            bool val = true;
            bool enviado = true;
            // Recuperar o email que está cadastrado nas configs.
            // Email abaixo está cadastrado no MailJet, provedor com 6000 msg/mês gratuitas
            //string email = "rika_alves@hotmail.com";  
            string email = _configuracaoService.FindAdminEmail();

            // Verifica se o nome foi digitado
            if (string.IsNullOrEmpty(namecontact))
            {
                ViewBag.ErroNome = "Campo nome não pode ser vazio ou nulo!";
                val = false;
            }
            // Verifica se o email foi digitado
            if (string.IsNullOrEmpty(emailcontact))
            {
                ViewBag.ErroEmail = "Campo e-mail não pode ser vazio ou nulo!";
                val = false;
            }
            // Caso o email foi digitado, verifica se ele está correto
            else
            {

                if (!Regex.IsMatch(emailcontact, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                {
                    ViewBag.ErroEmail = "E-mail digitado não está correto!";
                    val = false;
                }
            }
            // Verifica se o numero de telefone foi digitado
            if (string.IsNullOrEmpty(phonecontact))
            {
                ViewBag.ErroPhone = "Campo telefone não pode ser vazio ou nulo!";
                val = false;
            }
            // Verifica se o assunto foi digitado
            if (string.IsNullOrEmpty(subjectcontact))
            {
                ViewBag.ErroSubject = "Campo assunto não pode ser vazio ou nulo!";
                val = false;
            }

            // verifica se todas as validações foram feitas.
            if (val)
            {

                // Cria Instancia da classe enviar email
                //EnviarEmail env_mail = new EnviarEmail();

                // Se passou em todas as validações, realiza o envio de email
                var configEmail = _configuracaoService.FindEmail();
                var configSite = _configuracaoService.Find();
                bool flag = EnviarEmail.testeEmail(configEmail, email, email, subjectcontact, namecontact, phonecontact, "ModeloEmailContato", emailcontact, null, null, configSite.NomeSite);
                if (flag)
                {
                    ViewBag.Message = "Mensagem enviada!";
                    ViewBag.Enviado = true;
                    enviado = true;
                };
                // Caso não passe em alguma validação uma mensagem de erro será escrita
            }
            else
            {
                ViewBag.Message = "Mensagem não enviada!";
                ViewBag.Enviado = false;
                enviado = false;
            }

            // Se erro, Os campos continuam preenchidos para arrumar ou preencher
            if (!enviado)
            {
                ViewBag.NameContact = namecontact;
                ViewBag.EmailContact = emailcontact;
                ViewBag.PhoneContact = phonecontact;
                ViewBag.SubjectContact = subjectcontact;
            }
            // Se o email for enviado, os campos são zerados para um novo envio
            else
            {
                ViewBag.NameContact = "";
                ViewBag.EmailContact = "";
                ViewBag.PhoneContact = "";
                ViewBag.SubjectContact = "";

                var mail = new Malling();
                mail.Email = emailcontact;
                mail.Nome = namecontact;
                mail.Onde = "Contato";
                mail.DataCadastro = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));

                //Add malling
                if (!_mallingService.CheckMail(mail))
                {
                    _mallingService.AddMalling(mail);
                }
            }

            ViewBag.Redes = _socialMidia.FindAll();
            return View();

        }
    }
}