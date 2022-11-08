extends Node2D

func _ready():
	var scene = ResourceLoader.load("res://tilemap/Map.tscn").instance()
	$ViewportContainer/Viewport.add_child(scene)


func _unhandled_key_input(event):
	if Input.is_action_just_pressed("toggle_audio"):
		$AudioStreamPlayer.playing = !$AudioStreamPlayer.playing
