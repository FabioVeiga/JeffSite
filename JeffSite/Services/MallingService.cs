using System.Collections.Generic;
using System.Linq;
using JeffSite.Data;
using JeffSite.Models;

namespace JeffSite.Services
{
    public class MallingService
    {
        private readonly JeffContext _context;

         public MallingService(JeffContext context)
        {
            _context = context;
        }

        public void AddMalling(Malling mail){
            _context.Add(mail);
            _context.SaveChanges();
        }

        public bool CheckMail(Malling mail){
            var teste = _context.Mallings.FirstOrDefault(x => x.Email == mail.Email);
            if(teste == null){
                return false;
            }else{
                return true;
            }
        }

        public List<Malling> FillAllMalling(){
            return _context.Mallings.ToList();
        }

        public List<string> FillAllMallingJusEmail(){
            return _context.Mallings.Select(x => x.Email).ToList();
        }

        public void DeleteEmail(string email){
            var e = _context.Mallings.FirstOrDefault(x => x.Email == email);
            _context.Mallings.Remove(e);
            _context.SaveChanges();
        }
    }
}