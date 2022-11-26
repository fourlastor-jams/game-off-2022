extends StaticBody2D
tool


export(
    int,
    "LightGreen",
    "Green",
    "DarkGreen",
    "Purple",
    "LightPurple",
    "DarkPurple",
    "Red",
    "LightRed",
    "DarkRed",
    "Blue",
    "LightBlue",
    "DarkBlue",
    "LightYellow",
    "Yellow",
    "DarkYellow"
    ) var variant = 1;


# Called when the node enters the scene tree for the first time.
func _ready():
    $Sprite.frame = variant


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#    pass
