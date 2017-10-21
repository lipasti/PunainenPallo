using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

public class PunainenPallo : PhysicsGame
{
    Image pelaajaKuva = LoadImage("Pelaaja");
    PhysicsObject pelaaja;
    public override void Begin()
    {
        LuoPelaaja();


        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }
    void LuoPelaaja()
    {
        pelaaja = new PhysicsObject(50, 50, Shape.Circle);

        pelaaja.X = 100;
        pelaaja.Y = 100;
        pelaaja.Image = pelaajaKuva;

       
       Add(pelaaja);

    }
}
