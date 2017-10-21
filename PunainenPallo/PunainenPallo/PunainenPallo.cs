﻿using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

public class PunainenPallo : PhysicsGame
{
    int pelaajanNopeus = 200;
    int painovoima = 900;
    int hyppyVoima = 500;
    int pyorimisNopeus = 6;
    Image pelaajaKuva = LoadImage("Pelaaja");
    Image kuutioKuva = LoadImage("Kuutio");

    PlatformCharacter pelaaja;
    public override void Begin()
    {
        Gravity = new Vector(0, -painovoima);
        LuoKentta();
        AsetaTormaysKasittelijat();
        AsetaOhjaimet();
    }

    void AsetaTormaysKasittelijat()
    {
        AddCollisionHandler(pelaaja, "kuutio", PelaajaTormaaKuutioon);

    }

    private void PelaajaTormaaKuutioon(PhysicsObject pelaaja, PhysicsObject kuutio)
    {
        if( pelaaja.Bottom + 10 > kuutio.Top)
        {
            kuutio.Destroy();
        }
        else
        {
            pelaaja.Destroy();
        }
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
        ColorTileMap ruudut = ColorTileMap.FromLevelAsset("Level1");

        //2. Kerrotaan mitä aliohjelmaa kutsutaan, kun tietyn värinen pikseli tulee vastaan kuvatiedostossa.
        ruudut.SetTileMethod(Color.Red, LuoPelaaja);
        ruudut.SetTileMethod(Color.Black, LuoMaa);
        ruudut.SetTileMethod(Color.DarkGray, LuoKuutio);
        ruudut.SetTileMethod(Color.Orange, LuoLaatikko);


        //3. Execute luo kentän
        //   Parametreina leveys ja korkeus
        ruudut.Execute(50, 50);
    }

    private void LuoLaatikko(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject laatikko = new PhysicsObject(2*leveys, 2*korkeus);
        laatikko.Color = Color.Orange;
        laatikko.Position = paikka;
        Add(laatikko);
    }

    void LuoPelaaja(Vector paikka, double leveys, double korkeus)
    {
        pelaaja = new PlatformCharacter(leveys, korkeus, Shape.Circle);
        pelaaja.Position = paikka;
        pelaaja.Image = pelaajaKuva;
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

    void LiikutaPelaajaa(int x)
    {
        pelaaja.Walk(x * pelaajanNopeus);
        pelaaja.Angle += Angle.FromDegrees(-x*pyorimisNopeus);
    }

    void HyppaytaPelaajaa()
    {
        pelaaja.Jump(hyppyVoima);
    }
}
