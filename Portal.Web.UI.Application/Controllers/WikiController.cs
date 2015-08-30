using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Entities;
using Portal.Models.Wikis;
using AutoMapper;
using Portal.Attributes;

namespace Portal.Controllers
{
    public class WikiController : ApplicationController
    {

        // [Authorize]
        //public ActionResult Index() {



        //    var items = GetSession.QueryOver<Wiki>()
        //        .OrderBy(x=>x.Id).Desc
        //        .List();
        //    return View(items);
        //}

        [Authorize]
        public ActionResult Index(string letter = "all")
        {

            string alphabet = "אבגדהוזחטיכלמנסעפצקרשת";
           // string alphabetEn = "abcdefghijklmnopqrstuvwxyz";
            IList<SelectListItem> Letters = new List<SelectListItem>();
            //IList<SelectListItem> LettersEn = new List<SelectListItem>();

            Letters.Add(new SelectListItem()
            {
                Selected = true,
                Value = "all",
                Text = "כל האותיות"
            });
            //LettersEn.Add(new SelectListItem()
            //{
            //    Selected = true,
            //    Value = "all",
            //    Text = "כל האותיות"
            //});
            for (int i = 0; i < alphabet.Length; i++)
            {

                if (letter == alphabet[i].ToString())
                {
                    Letters.Add(new SelectListItem()
                    {
                        Selected = true,
                        Value = alphabet[i].ToString(),
                        Text = alphabet[i].ToString()
                    });
                }
                else
                {
                    Letters.Add(new SelectListItem()
                    {
                        Value = alphabet[i].ToString(),
                        Text = alphabet[i].ToString()
                    });

                }
            }

            //for (int i = 0; i < alphabetEn.Length; i++)
            //{
            //    if (letter == alphabetEn[i].ToString())
            //    {
            //        LettersEn.Add(new SelectListItem()
            //        {
            //            Selected = true,
            //            Value = alphabetEn[i].ToString(),
            //            Text = alphabetEn[i].ToString()
            //        });
            //    }
            //    else
            //    {
            //        LettersEn.Add(new SelectListItem()
            //        {

            //            Value = alphabetEn[i].ToString(),
            //            Text = alphabetEn[i].ToString()
            //        });

            //    }
            //}
            ViewData["letters"] = Letters;
         //   ViewData["lettersEn"] = LettersEn;
            ViewData["currentLetter"] = letter;

            if (letter != "all")
            {
                return View(GetSession.QueryOver<Wiki>().Where(x => x.Letter == letter).List());
            }
            else
            {
                return View(GetSession.QueryOver<Wiki>().List());

            }



        }

        [Authorize]
        public ActionResult Create(string letter)
        {
            string alphabet = "אבגדהוזחטיכלמנסעפצקרשת";

            var model = new WikiFormModel();

            if (alphabet.Contains(letter))
            {
                model = new WikiFormModel() { Date = DateTime.Now, Letter = letter };
            }
            //else
            //{
            //    model = new WikiFormModel() { Date = DateTime.Now, LetterEn = letter };
            //}

            return View(model);
        }


        [Authorize, HttpPost, Transaction, ValidateInput(false)]
        public ActionResult Create(WikiFormModel model)
        {
           
            var item = Mapper.Map<WikiFormModel, Wiki>(model);

            // int maxOrder = GetSession.Query<Wiki>().Where(x => x.Letter == item.Letter).Count();

            // item.Id = maxOrder + 1;
            GetSession.Save(item);

            return RedirectToAction("Index", new { model.Letter });
        }

        [Authorize]
        public ActionResult Edit(long id)
        {
            var item = GetSession.Get<Wiki>(id);

            var model = Mapper.Map<Wiki, WikiFormModel>(item);

            return View(model);
        }

        [HttpPost, Authorize, Transaction, ValidateInput(false)]
        public ActionResult Edit(WikiFormModel model)
        {
            var item = GetSession.Get<Wiki>(model.Id);

            Mapper.Map<WikiFormModel, Wiki>(model, item);
            GetSession.Update(item);

            return RedirectToAction("Index");
        }


        [Authorize, Transaction]
        public ActionResult Destroy(long id)
        {
            var item = GetSession.Get<Wiki>(id);
            GetSession.Delete(item);
            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public ActionResult WikiBox()
        {
            var items = GetSession.QueryOver<Wiki>()
                .Where(x => (x.Date >= DateTime.Now.Date))
                .OrderBy(x => x.Id).Desc
                .List();

            return PartialView(items);
        }

        public ActionResult List()
        {
            var items = GetSession.QueryOver<Wiki>()
                .OrderBy(x => x.Id).Asc
                .List();
            return View(items);
        }


        public ActionResult Show(string letter = "all")
        {

            string alphabet = "אבגדהוזחטיכלמנסעפצקרשת";
            //string alphabetEn = "zyxwvutsrqponmlkihgfedcba";
            
            ViewData["alphabet"] = alphabet;
           // ViewData["alphabetEn"] = alphabetEn;


            if (alphabet.Contains(letter))
            {
                var items = GetSession.QueryOver<Wiki>().
                    Where(x => x.Letter == letter)
                   .OrderBy(x => x.Id).Asc
                   .List();
            }

            if (letter == "all")
            {
                return View(GetSession.QueryOver<Wiki>().List());
            }
            else
            {
                
                    return View(GetSession.QueryOver<Wiki>().Where(x => x.Letter == letter).List());
                
                //else
                //{
                //    return View(GetSession.QueryOver<Wiki>().Where(x => x.LetterEn == letter).List());
                //}
            }
        }
    }
}
