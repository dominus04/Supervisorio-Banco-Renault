namespace Supervisório_Banco_Renault.Models
{
    public class OP20_AutomaticCommomR
    {
        public bool ScrapCage1 { get; set; }
        public bool ScrapCage2 { get; set; }
        public bool ScrapCage3 { get; set; }
        public bool ScrapCageNOK { get; set; }
        public bool ScrapCageFull { get; set; }

        public ushort ProductsOK { get; set; }
        public ushort ProductsNOK { get; set; }
    }
}
