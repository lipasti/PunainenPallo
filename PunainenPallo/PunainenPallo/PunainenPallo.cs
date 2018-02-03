using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

public class PunainenPallo : PhysicsGame
{
    int pelaajanNopeus = 500;
    int painovoima = 900;
    int hyppyVoima = 500;
    int pyorimisNopeus = 6;
    double pelaajaMaksiminopeus = 1000;
    int elamiaAluksi = 5;
    int kentta = 1;

    bool voiHypata = false;

    IntMeter elamat;


    Image pelaajaKuva = LoadImage("Pelaaja");
    Image kuutioKuva = LoadImage("Kuutio");
    Image maaliKuva = LoadImage("maali");
    private Vector aloitusPaikka;
    PhysicsObject pelaaja;

    public override void Begin()
    {
        Gravity = new Vector(0, -painovoima);
        LuoElamaLaskuri();
        LuoKentta();
        AsetaTormaysKasittelijat();
        AsetaOhjaimet();
    }

    private void LuoElamaLaskuri()
    {
        elamat = new IntMeter(elamiaAluksi);
        elamat.MinValue = 0;
        elamat.LowerLimit += GameOver;
        Label elamaNaytto = new Label();
        elamaNaytto.X = Screen.Left + 30;
        elamaNaytto.Y = Screen.Top - 30;
        elamaNaytto.TextColor = Color.Black;
        elamaNaytto.BindTo(elamat);
        Add(elamaNaytto);
    }

    private void GameOver()
    {
        ClearAll();
        kentta = 1;
        MessageDisplay.Add("Pelaaja hävisi pelin");
        Timer.SingleShot(5, Begin);
    }

    void AsetaTormaysKasittelijat()
    {
        AddCollisionHandler(pelaaja, "kuutio", PelaajaTormaaKuutioon);
        AddCollisionHandler(pelaaja, "maa", PelaajaOsuuMaahan);
        AddCollisionHandler(pelaaja, "laatikko", PelaajaOsuuMaahan);
        AddCollisionHandler(pelaaja, "maali", PelaajaOsuuMaaliin);
    }

    private void PelaajaOsuuMaaliin(PhysicsObject pelaaja, PhysicsObject maali)
    {
        SeuraavaanKenttaan();
    }

    private void SeuraavaanKenttaan()
    {
        kentta += 1;
        ClearAll();
        Begin();
    }

    private void PelaajaOsuuMaahan(PhysicsObject pelaaja, PhysicsObject maa)
    {
        voiHypata = true;
    }

    private void PelaajaTormaaKuutioon(PhysicsObject pelaaja, PhysicsObject kuutio)
    {
        if( pelaaja.Bottom + 10 > kuutio.Top)
        {
            kuutio.Destroy();
        }
        else
        {
            PelaajaKuoli();
        }
    }

    void PelaajaKuoli()
    {
        elamat.AddValue(-1);
        pelaaja.Position = aloitusPaikka;
    }

    void AsetaOhjaimet()
    {
        Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa oikealle", 1);
        Keyboard.Listen(Key.Right, ButtonState.Released, LiikutaPelaajaa, null, 0);
        Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa vasemmalle", -1);
        Keyboard.Listen(Key.Left, ButtonState.Released, LiikutaPelaajaa, null, 0);

        Keyboard.Listen(Key.Space, ButtonState.Pressed, HyppaytaPelaajaa, "Pelaaja hyppää");

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }
    
    void LuoKentta()
    {
        //1. Luetaan kuva uuteen ColorTileMappiin, kuvan nimen perässä ei .png-päätettä.
        ColorTileMap ruudut = ColorTileMap.FromLevelAsset("Level" + kentta);

        //2. Kerrotaan mitä aliohjelmaa kutsutaan, kun tietyn värinen pikseli tulee vastaan kuvatiedostossa.
        ruudut.SetTileMethod(Color.Red, LuoPelaaja);
        ruudut.SetTileMethod(Color.Black, LuoMaa);
        ruudut.SetTileMethod(Color.DarkGray, LuoKuutio);
        ruudut.SetTileMethod(Color.Orange, LuoLaatikko);
        ruudut.SetTileMethod(Color.Blue, LuoMaali);


        //3. Execute luo kentän
        //   Parametreina leveys ja korkeus
        ruudut.Execute(50, 50);
    }

    private void LuoLaatikko(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject laatikko = new PhysicsObject(2*leveys, 2*korkeus);
        laatikko.Color = Color.Orange;
        laatikko.Position = paikka;
        laatikko.Tag = "laatikko";
        Add(laatikko);
    }

    void LuoPelaaja(Vector paikka, double leveys, double korkeus)
    {
        aloitusPaikka = paikka;
        pelaaja = new PhysicsObject(leveys*0.75, korkeus*0.75, Shape.Circle);
        pelaaja.Position = paikka;
        pelaaja.Image = pelaajaKuva;
        pelaaja.MaxVelocity = pelaajaMaksiminopeus;
        Add(pelaaja);
    }

    void LuoMaa(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject palikka = PhysicsObject.CreateStaticObject(leveys, korkeus);
        palikka.Color = Color.JungleGreen;
        palikka.Position = paikka;
        palikka.Tag = "maa";
        Add(palikka);
    }

    void LuoKuutio(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject kuutio = new PhysicsObject(leveys, korkeus);
        kuutio.Color = Color.DarkGray;
        kuutio.Position = paikka;
        kuutio.Tag = "kuutio";
        kuutio.Image = kuutioKuva;
        Add(kuutio);
    }

    void LuoMaali(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject maali = PhysicsObject.CreateStaticObject(leveys, korkeus);
        maali.Color = Color.Blue;
        maali.Position = paikka;
        maali.Tag = "maali";
        maali.Image = maaliKuva;
        Add(maali);
    }

    void LiikutaPelaajaa(int x)
    {
        pelaaja.Push(new Vector(x * pelaajanNopeus, 0));
    }

    void HyppaytaPelaajaa()
    {
        if(voiHypata)
        {
            pelaaja.Hit(new Vector(0, hyppyVoima));
            voiHypata = false;
        }
    }
}
