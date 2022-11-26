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

func _ready():
    $Sprite.frame = variant
