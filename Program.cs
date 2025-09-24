using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.VisualBasic.Logging;


// Afmetingen Form bepalen
int hoogte = 0;
while (hoogte < 300 || hoogte > 900)
{
    Console.Write("Hoeveel pixels hoog is het scherm (300-900): ");
    string user_input = Console.ReadLine();

    // Checken of het een geldig getal is
    try
    {
        hoogte = int.Parse(user_input);
    }
    catch (Exception e)
    {
        Console.WriteLine($"'{user_input}' is geen geheel getal.");
    }
}


// Maak een Form aan
Form scherm = new Form();
scherm.Text = "Mandelbrot";
scherm.ClientSize = new Size(hoogte + 200, hoogte);

// Bitmap en Label aanmaken
int breedte_afb = hoogte - 20;
Bitmap plaatje = new Bitmap(breedte_afb, breedte_afb);
Label afbeelding = new Label();
scherm.Controls.Add(afbeelding);
afbeelding.Image = plaatje;
afbeelding.Location = new Point(210, 10);
afbeelding.Size = new Size(breedte_afb, breedte_afb);


// Buttons aanmaken
Button knop = new Button();
scherm.Controls.Add(knop);
knop.Location = new Point(20, 230);
knop.Text = "GO";
knop.Size = new Size(120, 50);


// Beginwaardes
double schaal = 3.0 / breedte_afb;
double x = -0.6, y = 0.0;
int max = 300;
int baseMax = max;

int rood_multiplier = 1;
int groen_multiplier = 2;
int blauw_multiplier = 3;


// Tekstbox maken
TextBox maak_textbox(int x, int y, int breedte, int hoogte)
{
    TextBox naam = new TextBox();
    scherm.Controls.Add(naam);
    naam.Location = new Point(x, y);
    naam.Size = new Size(breedte, hoogte);
    return naam;
}

TextBox tekstbox_schaal = maak_textbox(70, 80, 130, 50);
TextBox tekstbox_x = maak_textbox(70, 120, 130, 50);
TextBox tekstbox_y = maak_textbox(70, 160, 130, 50);
TextBox tekstbox_max = maak_textbox(70, 200, 130, 50);


// text voor de tekstbox maken
Label maak_label(int x, int y, string tekst)
{
    Label naam = new Label();
    scherm.Controls.Add(naam);
    naam.Location = new Point(x, y);
    naam.Text = tekst;
    return naam;
}

Label schaaltekst = maak_label(10, 80, "schaal:");
Label middenxtekst = maak_label(10, 120, "midden x:");
Label middenytekst = maak_label(10, 160, "midden y:");
Label maxtekst = maak_label(10, 200, "iteraties:");


//Slider maken
TrackBar slider_rood = new TrackBar();
scherm.Controls.Add(slider_rood);
slider_rood.Minimum = 0;
slider_rood.Maximum = 255;
slider_rood.Value = rood_multiplier;
slider_rood.TickFrequency = 1;
slider_rood.Location = new Point(30, 30);




//plaatje en textboxen updaten
void update()
{
    generate(x, y, schaal, max);
    afbeelding.Invalidate();

    tekstbox_schaal.Text = $"{schaal}";
    tekstbox_schaal.Invalidate();

    tekstbox_x.Text = $"{x}";
    tekstbox_x.Invalidate();

    tekstbox_y.Text = $"{y}";
    tekstbox_y.Invalidate();

    tekstbox_max.Text = $"{max}";
    tekstbox_max.Invalidate();
}

void go(object o, EventArgs e)
{
    List a = [tekstbox_schaal, tekstbox_x, tekstbox_y, tekstbox_max];
    
    try
    {
        max = int.Parse(tekstbox_max.Text);
    }
    catch (Exception)
    {
        tekstbox_max.BackColor = Color.Red;
    }

    if 
    update();
}

knop.Click += go;

// Berekent het Mandelgetal van punt (x, y) en geeft reele en imaginaire deler (a en b) terug voor smoothcolouring
 (double, double, int) mandelgetal(double x, double y, int max)
{
    double a = 0, b = 0;// start (a,b) = (0,0)
    int t = 0;
    while ((a * a + b * b) <= 4 && t < max) // Stopt als de afstand groter is dan 2 of als "max" is bereikt
    {
        // Bereken nieuwe (a, b)
        double an = a * a - b * b + x;
        double bn = 2 * a * b + y;

        // Vervang oude (a, b) met nieuwe ("an", "bn")
        a = an;
        b = bn;
        t++;
    }
    return (a, b, t);
}

// Zet pixel coördinaten om in wiskundige coördinaten
(double, double) coördinaat(int px, int py, double x_min, double x_max, double y_min, double y_max)
{
    double x = x_min + px * (x_max - x_min) / (breedte_afb - 1);
    // (x_max - x_min) vertelt hoe breed de x-as "wiskundig" moet zijn
    // en gedeeld door (breedte_afb - 1) schaal je dat naar het aantal pixels

    double y = y_max - py * (y_max - y_min) / (breedte_afb - 1); // Hier doen we - i.p.v. +, omdat pixels boven beginnen
    return (x, y);
}

// Genereer de mandelbrot met een bepaald middenpunt
void generate(double x, double y, double schaal, int max)
{
    double domein = breedte_afb * schaal;

    double x_min = x - 0.5 * domein;
    double x_max = x + 0.5 * domein;
    double y_min = y - 0.5 * domein;
    double y_max = y + 0.5 * domein;

    for (int px = 0; px < breedte_afb; px++)   // Ga langs alle x coördinaten
    {
        for (int py = 0; py < breedte_afb; py++) // Ga langs alle y coördinaten
        {
            (double x2, double y2) = coördinaat(px, py, x_min, x_max, y_min, y_max);
            var (a, b, m_getal) = mandelgetal(x2, y2, max);      // Bereken het mandelgetal van deze pixel + geef a en b mee

            if (m_getal == max)                        // Check of het mandelgetal groter is dan max
                plaatje.SetPixel(px, py, Color.Black);
            else
            {
                // -----Eenvoudige kleurtoekenning (niet meer gebruikt)-----
                //int kleurwaarde = m_getal;
                //Color kleur = Color.FromArgb(kleurwaarde % 256, kleurwaarde % 256, kleurwaarde % 256);
                //plaatje.SetPixel(px, py, kleur);

                double afstand_oorsprong_punt = Math.Sqrt(a * a + b * b);
                double smooth_kleurwaarde = m_getal + 1- Math.Log(Math.Log(afstand_oorsprong_punt)) / Math.Log(2.0);
                int kleurwaarde = (int)(360*smooth_kleurwaarde/max);
                Color kleur = Color.FromArgb(kleurwaarde * rood_multiplier % 256, kleurwaarde * groen_multiplier % 256, kleurwaarde * blauw_multiplier % 256);
                plaatje.SetPixel(px, py, kleur);
            }

        }
    }
}



// Registreer muis inputs en zoom in of uit
void muisKlik(object s, MouseEventArgs ea)
{
    double domein = breedte_afb * schaal; // Assenstelsel lengte van het domein/bereik

    // Min en max van x en y berekenen m.b.v. domein en middelpunt
    double x_min = x - domein / 2;
    double x_max = x + domein / 2;
    double y_min = y - domein / 2;
    double y_max = y + domein / 2;

    // Bereken (x, y) van klik en zet daar het middelpunt
    (double x_klik, double y_klik) = coördinaat(ea.X, ea.Y, x_min, x_max, y_min, y_max);

    x = x_klik;
    y = y_klik;

    // Zoom in of uit
    if (ea.Button == MouseButtons.Left)
    {
        schaal *= 0.5;
        max = (int)(baseMax * Math.Log(1.0 / (120 * schaal)));   // past max iteraties logaritmisch aan zodat detail bij inzoomen bewaart blijft
    }
    if (ea.Button == MouseButtons.Right)
        {
            schaal *= 2.0;
            max = (int)(baseMax * Math.Log(1.0 / (120 * schaal)));  
        }
    update();
}

afbeelding.MouseClick += muisKlik;

update();
Application.Run(scherm);