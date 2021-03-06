﻿using System;
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
    double pelaajaMaksiminopeus = 2000;
    int elamiaAluksi = 5;
    int boostejaAluksi = 0;
    int kentta = 1;
    double pallonMassa = 1;
    bool voiHypata = false;
    Timer hyppyVoimaLaskuri;
    double hyppyVoimanKeraysAika = 2.0;
    private Vector teleportKohde;

    IntMeter elamat;
    IntMeter boost;


    Image pelaajaKuva = LoadImage("Pelaaja");
    Image kuutioKuva = LoadImage("Kuutio");
    Image maaliKuva = LoadImage("maali");
    Image sydanKuva = LoadImage("sydan");
    Image boostkuva = LoadImage("boost");
    private Vector aloitusPaikka;
    PhysicsObject pelaaja;

    public override void Begin()
    {
        Gravity = new Vector(0, -painovoima);
        LuoElamaLaskuri();
        LuoBoostLaskuri();
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
    private void LuoBoostLaskuri()
    {
        boost = new IntMeter(boostejaAluksi);
        boost.MinValue = 0;
        Label boostNaytto = new Label();
        boostNaytto.X = Screen.Left + 100;
        boostNaytto.Y = Screen.Top - 30;
        boostNaytto.TextColor = Color.Black;
        boostNaytto.BindTo(boost);
        Add(boostNaytto);
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
        AddCollisionHandler(pelaaja, "sydan", PelaajaOsuuSydameen);
        AddCollisionHandler(pelaaja, "teleport", PelaajaOsuuTeleportiin);
        AddCollisionHandler(pelaaja, "boost", PelaajaOsuuBoostiin);

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

    private void PelaajaOsuuSydameen(PhysicsObject pelaaja, PhysicsObject sydan)
    {
        if(elamat.Value < elamiaAluksi)
        {
            elamat.AddValue(1);
        }
        sydan.Destroy();
    }
    private void PelaajaOsuuBoostiin(PhysicsObject pelaaja, PhysicsObject boosti)
    {
        boost.AddValue(1);
        boostejaAluksi = boost.Value;

        boosti.Destroy();
    }

    private void PelaajaOsuuTeleportiin(PhysicsObject pelaaja, PhysicsObject teleport)
    {
        pelaaja.Position = teleportKohde;
    }

    void PelaajaKuoli()
    {
        elamat.AddValue(-1);
        pelaaja.Position = aloitusPaikka;
    }

    void AsetaOhjaimet()
    {
        Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa oikealle", 2);
        Keyboard.Listen(Key.Right, ButtonState.Released, LiikutaPelaajaa, null, 0);
        Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa vasemmalle", -2);
        Keyboard.Listen(Key.Left, ButtonState.Released, LiikutaPelaajaa, null, 0);
        Keyboard.Listen(Key.R, ButtonState.Pressed, AloitaKenttaAlusta, "Aloita kenttä alusta");
        Keyboard.Listen(Key.Space, ButtonState.Pressed, KeraaHyppyvoimaa, null);
        Keyboard.Listen(Key.Space, ButtonState.Released, HyppaytaPelaajaa, "Pelaaja hyppää");
        Keyboard.Listen(Key.B, ButtonState.Pressed, KaytaBoost, "PelaajaKäyttääBoostin");

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }
    void AloitaKenttaAlusta()
    {
        pelaaja.Position = aloitusPaikka;
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
        ruudut.SetTileMethod(Color.Pink, LuoSydan);
        ruudut.SetTileMethod(Color.Green, LuoTeleport);
        ruudut.SetTileMethod(Color.LightGreen, LuoTeleportKohde);
        ruudut.SetTileMethod(Color.Gold, LuoBoost);


        //3. Execute luo kentän
        //   Parametreina leveys ja korkeus
        ruudut.Execute(50, 50);
        Camera.Follow(pelaaja);
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
        pelaaja = new PhysicsObject(leveys*0.50, korkeus*0.50, Shape.Circle);
        pelaaja.Position = paikka;
        pelaaja.Image = pelaajaKuva;
        pelaaja.MaxVelocity = pelaajaMaksiminopeus;
        pelaaja.Mass = pallonMassa;
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

    void LuoSydan(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject sydan = PhysicsObject.CreateStaticObject(leveys, korkeus);
        sydan.Color = Color.Pink;
        sydan.Position = paikka;
        sydan.Tag = "sydan";
        sydan.Image = sydanKuva;
        Add(sydan);
    }
    private void LuoTeleport(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject teleport = PhysicsObject.CreateStaticObject(leveys, korkeus);
        teleport.Color = Color.Green;
        teleport.Position = paikka;
        teleport.Tag = "teleport";
        Add(teleport);
    }
    private void LuoBoost (Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject boost = PhysicsObject.CreateStaticObject(leveys, korkeus);
        boost.Color = Color.Green;
        boost.Position = paikka;
        boost.Tag = "boost";
        boost.Image = boostkuva;
        Add(boost);
    }
    private void LuoTeleportKohde(Vector paikka, double leveys, double korkeus)
    {
        teleportKohde = paikka;
    }

    void LiikutaPelaajaa(int x)
    {
        pelaaja.Push(new Vector(x * pelaajanNopeus, 0));
    }

    void KeraaHyppyvoimaa()
    {
        hyppyVoimaLaskuri = new Timer();
        hyppyVoimaLaskuri.Start();
    }

    void HyppaytaPelaajaa()
    {
        double aikaaKulunut = hyppyVoimaLaskuri.SecondCounter.Value;
        hyppyVoimaLaskuri.Stop();
        if (voiHypata)
        {
            pelaaja.Hit(new Vector(0, LaskeHyppyVoima(aikaaKulunut)));
            voiHypata = false;
        }
    }

    double LaskeHyppyVoima(double aikaaKulunut)
    {
        return Math.Max(0, hyppyVoima * (1 - (aikaaKulunut / hyppyVoimanKeraysAika)));
    }
    void KaytaBoost()
    {
        if (boost.Value < 1) return;
        
       boost.AddValue(-1);
        boostejaAluksi = boost.Value;
       SeuraavaanKenttaan();
    }
}
