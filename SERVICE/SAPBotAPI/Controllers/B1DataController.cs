using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAPBotAPI.Models;

namespace SAPBotAPI.Controllers
{
    public class B1DataController : ApiController
    {
        /// <summary>
        /// Gets the top 5 opportunities sorted descending on percentage of profit
        /// </summary>
        /// <returns></returns>
        public BOTReply GetOpportunities()
        {
            return GetOpportunities(5);
        }
       
        /// <summary>
        /// Gets the top N opportunities sorted ascending/descending on percentage of profit
        /// </summary>
        /// <param name="Count">Number of opportunities to return</param>
        /// <param name="Sort">Sort order (Asc/Desc. Default is Desc)</param>
        /// <returns></returns>
        public BOTReply GetOpportunities(int? Count, string Sort = "DESC")
        {
            BOTReply reply = new BOTReply();
            Reply rp = new Reply();
            rp.type = "list";
            using (SBODemoSGEntities data = new SBODemoSGEntities())
            {
                var oops = data.OOPRs.Where(x => x.Status == "O");

                int max = 5;
                if ((Count ?? 0) > 0)
                    max = Count.Value;

                if (Sort.ToUpper() == "ASC")
                    oops = oops.OrderBy(x => x.PrcnProf).ThenBy(y => y.CloPrcnt).Take(max);
                else
                    oops = oops.OrderByDescending(x => x.PrcnProf).ThenBy(y => y.CloPrcnt).Take(max);

                foreach (var item in oops)
                {
                    rp.content.elements.Add(new Element()
                    {
                        title = $"#{item.OpprId} - ({item.CloPrcnt}%) / {item.WtSumLoc} SGD (Weighed Amount)",
                        subtitle = $"{item.CardCode} - {item.CardName} {Environment.NewLine} {item.SumProfL} SGD (Total profit) {Environment.NewLine} Final Negotiation"
                    });
                }
                reply.replies.Add(rp);
            }

            return reply;
        }

        /// <summary>
        /// Gets the top 5 products sorted descending on 'On Hand'
        /// </summary>
        /// <returns></returns>
        public BOTReply GetProducts()
        {
            return GetProducts(5);
        }

        /// <summary>
        /// Gets the N number of products sorted descending on 'On Hand'.
        /// </summary>
        /// <param name="Count">Number of products</param>
        /// <param name="Descr">Description to search</param>
        /// <returns></returns>
        public BOTReply GetProducts(int? Count, string Descr = null)
        {
            BOTReply reply = new BOTReply();
            Reply rp = new Reply();
            rp.type = "list";
            using (SBODemoSGEntities data = new SBODemoSGEntities())
            {
                var prods = data.OITMs
                    .Join(data.ITM1s,
                            oitm => new { icode = oitm.ItemCode, pricel = 1 },
                            itm1 => new { icode = itm1.ItemCode, pricel = (int)itm1.PriceList },
                            (oitm, itm1) => new { oitm, itm1})
                    .Where(rec => rec.oitm.InvntItem == SAP_YesNo.Yes && rec.oitm.frozenFor == SAP_YesNo.No);

                int max = 5;
                if ((Count ?? 0) > 0)
                    max = Count.Value;

                if (!string.IsNullOrWhiteSpace(Descr))
                    prods = prods.Where(x => x.oitm.ItemName.Contains(Descr));

                prods = prods.OrderByDescending(x => x.oitm.OnHand).Take(max);

                foreach (var item in prods)
                {
                    rp.content.elements.Add(new Element()
                    {
                        title = $"{item.oitm.ItemCode}: {item.oitm.ItemName}",
                        subtitle = $"In Stock: {item.oitm.OnHand} - Price: S$ {item.itm1.Price}"
                    });
                }
                reply.replies.Add(rp);
            }

            return reply;
        }

        /// <summary>
        /// Get top 5 customers 
        /// </summary>
        /// <returns></returns>
        public BOTReply GetCustomers()
        {
            return GetBusinessPartner(5);
        }

        /// <summary>
        /// Top N customers
        /// </summary>
        /// <param name="Count">Number of customers</param>
        /// <param name="Name">Name to search</param>
        /// <returns></returns>
        public BOTReply GetCustomers(int? Count, string Name = null)
        {
            return GetBusinessPartner(Count, Name);
        }


        /// <summary>
        /// Get top 5 vendors 
        /// </summary>
        /// <returns></returns>
        public BOTReply GetVendors()
        {
            return GetBusinessPartner(5, BPType:"Vend");
        }

        /// <summary>
        /// Top N vendors
        /// </summary>
        /// <param name="Count">Number of vendors</param>
        /// <param name="Name">Name to search</param>
        /// <returns></returns>
        public BOTReply GetVendors(int? Count, string Name = null)
        {
            return GetBusinessPartner(Count, Name, "Vend");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Count"></param>
        /// <param name="Name"></param>
        /// <param name="BPType"></param>
        /// <returns></returns>
        private BOTReply GetBusinessPartner(int? Count, string Name = null, string BPType = "Cust")
        {
            BOTReply reply = new BOTReply();
            Reply rp = new Reply();
            rp.type = "list";
            using (SBODemoSGEntities data = new SBODemoSGEntities())
            {
                var Bps = data.OCRDs.Where(x => x.frozenFor == SAP_YesNo.No); ;

                int max = 5;
                if ((Count ?? 0) > 0)
                    max = Count.Value;

                if (BPType == "Cust")
                    Bps = Bps.Where(x => x.CardType == "C");
                else
                    Bps = Bps.Where(x => x.CardType == "S");

                if (!string.IsNullOrWhiteSpace(Name))
                {
                    Bps = Bps.Where(x => x.CardName.Contains(Name));
                }

                Bps = Bps.Take(max);

                foreach (var item in Bps)
                {
                    rp.content.elements.Add(new Element()
                    {
                        title = $"{item.CardCode}: {item.CardName}",
                        subtitle = $"Balance S$: {item.BalanceSys} - Email: {item.E_Mail}"
                    });
                }
                reply.replies.Add(rp);

            }
            return reply;
        }

    }

   
}
