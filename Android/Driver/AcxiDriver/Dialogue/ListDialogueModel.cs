using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcxiDriver.Adapter;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace AcxiDriver.Dialogue
{
    public class ListDialogueModel : Android.Support.V4.App.DialogFragment
    {

        private ListView mListView;
        private List<string> ListOfModel = new List<string>();
        TextInputLayout txtwhich;
        string mmake = "";
        public ListDialogueModel(TextInputLayout txtsender, string make)
        {
            txtwhich = txtsender;
            mmake = make;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.stringdialogue, container, false);

            InitCarList(mmake);
            TextView txtheader = (TextView)view.FindViewById(Resource.Id.txtDialog_header);
            txtheader.Text = "Select Model";
            mListView = view.FindViewById<ListView>(Resource.Id.Listview1);
            StringListAdapter adapter = new StringListAdapter(Application.Context, ListOfModel);
            mListView.Adapter = adapter;
            mListView.ItemClick += MListView_ItemClick;

            return view;
        }

        private void MListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            txtwhich.EditText.Text = ListOfModel[e.Position];
            // txtwhich.EditText.Tag = ListBankCodes[e.Position];
            this.Dismiss();
        }

        public void InitCarList(string car_make)
        {
            string c_toyota = "Toyota";
            string[] toyota = {"4-Runner", "Avalon", "Avanza", "Avensis","Brevis","Camry","Carina","Celica","Corolla","Corona","Echo","FJ Cruiser","Fortuner"
                    ,"HiAce","Highlander","Hilux","Ipsum","Land Cruiser","Prado","Matrix","Previa","Prius","RAV4","Sequoia","Sienna","Solara","Tacoma","Tundra","Venza","Yaris"};

            if (c_toyota == car_make)
            {
                ListOfModel = toyota.ToList<string>();
                return;
            }

            string c_honda = "Honda";
            string[] honda = {"Accord", "Acty","City","Civic","CL","CR","Crossroad","Crosstour","CR-V","CRX","CR-Z","CT","CX","Element","Express",
                "Insight","Jazz","Lead","Legend","Odyssey","Passport","Pilot","Prelude","Ridgeline","Shuttle","Stream","TL","XL"};

            if (c_honda == car_make)
            {
                ListOfModel = honda.ToList<string>();
                return;
            }


            string c_nissan = "Nissan";
            string[] nissan = {"Almera","Armada","Bluebird","Caravan","Crew","Frontier","Gloria","Hardbody","Juke","Laurel","Liberty","March","Maxima","Murano",
            "pathfinder","Primastar","Primera","Qashqai","Quest","Rogue","Santana","Sentra","Serena","Skyline","Teana","Tiida","Titan","Versa","Xterra","X-Trail"};

            if (c_nissan == car_make)
            {
                ListOfModel = nissan.ToList<string>();
                return;
            }

            string c_mecercedes = "Mercedes-Benz";
            string[] mercedes = { "E-class", "C-class", "S-class", "M-class", "Sprinter" };

            if (c_mecercedes == car_make)
            {
                ListOfModel = mercedes.ToList<string>();
                return;
            }


            string c_hyundai = "Hyundai";
            string[] hyundai = {"Accent","Atos","Azera","County","Creta","Elantra","Entourage","Equus","Genesis","Grandeur","ix35","Lantra","Santa Fe",
                "Sonata","Stellar","Tuscon","Veloster","Veracruz","Verna"};

            if (c_hyundai == car_make)
            {
                ListOfModel = hyundai.ToList<string>();
                return;
            }

            string c_mitsubishi = "Mitsubishi";
            string[] mitsubishi = {"ASX","Canter","Carisma","Challenger","Diamante","Eclipse","Endeavor","Galant","Jeep","Lancer","Mirage","Montero","Outlander","Pajero"
                    ,"Raider","Sigma","SpaceRunner","Spacestar","Spacewagon"};

            if (c_mitsubishi == car_make)
            {
                ListOfModel = mitsubishi.ToList<string>();
                return;
            }

            //FORD
            string c_ford = "Ford";
            string[] ford = {"Aerostar","Consul","Contour","Cougar","Courier","EcoSport","Edge","Escort","Everest","Excursion"
                    ,"Expedition","Explorer","F-150","Fiesta","Flex","Focus","Freestyle","Fusion","Mustang","Ranger","Taurus","Urban"};

            if(c_ford == car_make)
            {
                ListOfModel = ford.ToList<string>();
                return;
            }


            //AUDI  

            string c_audi = "Audi";
            string[] audi = { "90", "A2", "A4", "A5", "A6", "A7", "A8", "Q5", "Q7", "Quattro", "R8", "S6", "TT" };
            if(c_audi == car_make)
            {
                ListOfModel = audi.ToList<string>();
                return;
            }


            // KIA
            string c_kia = "Kia";
            string[] kia = { "Cerato", "Opirus", "Optima", "Picanto", "Sedona", "Sephia", "Sorento", "Soul", "Spectra", "Sportage" };
            
            if(c_kia == car_make)
            {
                ListOfModel = kia.ToList<string>();
            }

            //VOLKSWAGEN
            string c_volks = "Volkswagen";
            string[] volkswagen = { "Beetle", "Golf", "Jetta", "Multivan", "Passat", "Pickup", "Routan", "Sharan", "Tiguan", "Touareg", "Touran", "Transporter", "Vento" };

            if (c_volks == car_make)
            {
                ListOfModel = volkswagen.ToList<string>();
                return;
            }

            // BMW
            string c_bmw = "BMW";
            string[] BMW = { "3 Series", "5 Series", "7 Series", "8 Series", "G-Series", "M3", "S-Series", "X1", "X3", "X5", "X6", "Z4" };

            if(c_bmw == car_make)
            {
                ListOfModel = BMW.ToList();
                return;
            }

            //ACURA
            string c_acura = "Acura";
            string[] acura = { "CL", "ILX", "MDX", "RDX", "TL", "TSX", "ZDX" };

            if(c_acura == car_make)
            {
                ListOfModel = acura.ToList();
                return;
            }

            //LEXUS
            string c_lexus = "Lexus";
            string[] lexus = { "ES330", "ES350", "RX330", "RX350", "IS", "GS" };

            if(c_lexus == car_make)
            {
                ListOfModel = lexus.ToList();
                return;
            }

        }
    }
}