using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

public class PunainenPallo : PhysicsGame
{
    int pelaajanKoko = 70;
    int pelaajanNopeus = 200;
    Vector pelaajanAloituspaikka = new Vector(100, 100);
    Image pelaajaKuva = LoadImage("Pelaaja");

    PhysicsObject pelaaja;
    public override void Begin()
    {
        LuoPelaaja();

        Keyboard.Listen(Key.Up, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa ylös", new Vector(0, pelaajanNopeus));
        Keyboard.Listen(Key.Up, ButtonState.Released, LiikutaPelaajaa, null, Vector.Zero);
        Keyboard.Listen(Key.Down, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa alas", new Vector(0, -pelaajanNopeus));
        Keyboard.Listen(Key.Down, ButtonState.Released, LiikutaPelaajaa, null, Vector.Zero);
        Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa oikealle", new Vector(pelaajanNopeus, 0));
        Keyboard.Listen(Key.Right, ButtonState.Released, LiikutaPelaajaa, null, Vector.Zero);
        Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaPelaajaa, "Pelaajaa vasemmalle", new Vector(-pelaajanNopeus, 0));
        Keyboard.Listen(Key.Left, ButtonState.Released, LiikutaPelaajaa, null, Vector.Zero);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }
    void LuoPelaaja()
    {
        pelaaja = new PhysicsObject(pelaajanKoko, pelaajanKoko, Shape.Circle);
        pelaaja.Position = pelaajanAloituspaikka;
        pelaaja.Image = pelaajaKuva;
        Add(pelaaja);
    }

    void LiikutaPelaajaa(Vector suunta)
    {
        pelaaja.Move(suunta);
    }
}
