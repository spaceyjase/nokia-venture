extends Node

var music:AudioStream = preload("res://data/music/tune.wav")

func _ready() -> void:
  AudioManager.play_music(music)

func _input(event):
  if event.is_action_pressed('ui_select'):
    get_tree().change_scene("res://data/scenes/Main.tscn")
