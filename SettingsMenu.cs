using Godot;
using System;

public partial class SettingsMenu : Control
{
	private HSlider _volumeSlider;
	private CheckButton _fullscreenToggle;
	private Button _backButton;
	private int _masterBusIndex;

	public override void _Ready()
	{
		// Obtém as referências dos nós da cena
		_volumeSlider = GetNode<HSlider>("VBoxContainer/VolumeSlider");
		_fullscreenToggle = GetNode<CheckButton>("VBoxContainer/FullscreenCheckButton");
		_backButton = GetNode<Button>("VBoxContainer/BackButton");

		// Configura o índice do canal de áudio principal
		_masterBusIndex = AudioServer.GetBusIndex("Master");

		// Sincroniza os controles visuais com o estado atual do jogo
		_fullscreenToggle.ButtonPressed = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen;
		_volumeSlider.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_masterBusIndex));

		// Conecta os eventos (Signals) via código usando expressões lambda
		_volumeSlider.ValueChanged += OnVolumeSliderValueChanged;
		_fullscreenToggle.Toggled += OnFullscreenCheckButtonToggled;
		_backButton.Pressed += OnBackButtonPressed;
	}

	private void OnVolumeSliderValueChanged(double value)
	{
		float linearValue = (float)value;
		AudioServer.SetBusVolumeDb(_masterBusIndex, Mathf.LinearToDb(linearValue));
	}

	private void OnFullscreenCheckButtonToggled(bool toggledOn)
	{
		if (toggledOn)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
		else
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
	}

	private void OnBackButtonPressed()
	{
		Hide(); // Oculta o menu (ou use GetTree().ChangeSceneToFile(...) se for outra cena)
	}
}
