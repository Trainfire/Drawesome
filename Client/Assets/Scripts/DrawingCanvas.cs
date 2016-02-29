using UnityEngine;
using System.Collections;
using Protocol;
using System;

public class DrawingCanvas : MonoBehaviour, Game.IGameStateHandler
{
    UiDrawingCanvas Canvas { get; set; }
    Client Client { get; set; }

    uint BrushSize { get; set; }

    public void Initialise(Client client, UiDrawingCanvas view)
    {
        Client = client;
        Canvas = view;
        SetBrushSize(SettingsLoader.Settings.BrushSize);
    }

    public byte[] GetEncodedImage()
    {
        return Canvas.Texture.EncodeToPNG();
    }

    public void SetBrushSize(int size)
    {
        Canvas.BrushSize = (int)Mathf.Clamp(size, 1f, 5f);
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
        var image = Convert.FromBase64String(drawing.Image);
        SetImage(image);
    }

    public void Clear()
    {
        var pixels = Canvas.Texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        Canvas.Texture.SetPixels(pixels);
        Canvas.Texture.Apply();
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

    void LateUpdate()
    {
        float delta = Input.GetAxis("Mouse ScrollWheel");

        if (delta > 0.1f)
        {
            Canvas.BrushSize++;
        }
        else if (delta < -0.1f)
        {
            Canvas.BrushSize--;
        }

        SetBrushSize(Canvas.BrushSize);
    }
}
