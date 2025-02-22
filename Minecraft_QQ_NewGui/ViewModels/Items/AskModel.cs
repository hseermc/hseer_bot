﻿using CommunityToolkit.Mvvm.ComponentModel;
using Minecraft_QQ_Core;
using Minecraft_QQ_Core.Config;
using System.Collections.Generic;

namespace Minecraft_QQ_NewGui.ViewModels.Items;

public partial class AskModel(WindowModel top, KeyValuePair<string, string> item) : ObservableObject
{
    public KeyValuePair<string, string> Obj { get; } = item;

    [ObservableProperty]
    private string? _check = item.Key;

    [ObservableProperty]
    private string? _res = item.Value;

    partial void OnCheckChanged(string? oldValue, string? newValue)
    {
        if (string.IsNullOrWhiteSpace(oldValue))
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(newValue))
        {
            Check = Obj.Key;
            return;
        }

        Minecraft_QQ.Asks.AskList.Remove(oldValue);
        Minecraft_QQ.Asks.AskList.Add(Check!, Res!);
        ConfigWrite.Ask();

        top.ShowNotify("已设置自动应答");
    }

    partial void OnResChanged(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Res = Obj.Value;
            return;
        }

        Minecraft_QQ.Asks.AskList[Check!] = Res!;
        ConfigWrite.Ask();

        top.ShowNotify("已设置自动应答");
    }
}
