extends Node


var music:AudioStream = preload("res://data/music/jingle1.wav")


func _ready() -> void:
  AudioManager.play_music(music)

func _input(event):
  if event.is_action_pressed('ui_select'):
    Global.NewGame()
    AudioManager.stop_music()
