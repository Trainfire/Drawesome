using UnityEngine;
using System.Collections;
using Protocol;
using System;

public class DrawingCanvas : Game.IGameStateHandler
{
    UiDrawingCanvas Canvas { get; set; }
    Client Client { get; set; }

    public DrawingCanvas(UiDrawingCanvas view)
    {
        Canvas = view;
        AllowDrawing = false;
    }

    public DrawingCanvas(Client client, UiDrawingCanvas view) : this(view)
    {
        Client = client;
    }

    public byte[] GetEncodedImage()
    {
        return Canvas.Texture.EncodeToPNG();
    }

    public void SetBrushColor(uint colorId)
    {
        int index = (int)colorId;
        Canvas.BrushColor = Canvas.PlayerColors[index].Color;
    }

    public void SetImage(byte[] bytes)
    {
        Canvas.Texture.LoadImage(bytes);
        Canvas.Texture.Apply();
    }

    public void SetImage(DrawingData drawing)
    {
        SetBrushColor(drawing.Creator.RoomId);
        SetImage(drawing.Image);
    }

    public void Recolor(Color color)
    {
        var pixels = Canvas.Texture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i] != Color.white)
                pixels[i] = color;
        }

        Canvas.Texture.SetPixels(pixels);
        Canvas.Texture.Apply();
    }

    public bool AllowDrawing
    {
        get
        {
            return Canvas.AllowDrawing;
        }
        set
        {
            Canvas.AllowDrawing = value;
        }
    }

    void Game.IGameStateHandler.HandleState(GameState state)
    {
        switch (state)
        {
            case GameState.PreGame:
                AllowDrawing = false;
                break;
            case GameState.Drawing:
                SetBrushColor(Client.PlayerData.RoomId);
                AllowDrawing = true;
                break;
            case GameState.Answering:
                AllowDrawing = false;
                break;
        }
    }
}
