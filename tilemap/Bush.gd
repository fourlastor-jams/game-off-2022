extends StaticBody2D
tool


export(
    int,
    "LightGreen",
    "Green",
    "DarkGreen",
    "LightPurple",
    "Purple",
    "DarkPurple",
    "LightRed",
    "Red",
    "DarkRed",
    "LightBlue",
    "Blue",
    "DarkBlue",
    "Yellow",
    "LightYellow",
    "DarkYellow"
    ) var variant = 5

func _ready():
    $Sprite.frame = variant
