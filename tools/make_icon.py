# Builds icon.png: the game's chest ping sprite (texInventoryIconOutlined, the one MapChests uses for chests) over a
# dark gradient, with "MAP" / "CHESTS" below in the style of Kaitorque's other mods. Needs Python with UnityPy and Pillow.
# Usage: python tools/make_icon.py
import os
import UnityPy
from PIL import Image, ImageChops, ImageDraw, ImageFont

GAME_AA = r"E:\SteamLibrary\steamapps\common\Risk of Rain 2\Risk of Rain 2_Data\StreamingAssets\aa"
BUNDLE_HINT = "ror2-base-common-miscicons"
SPRITE = "texInventoryIconOutlined"
REPO = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SIZE = 256


def load_sprite():
    for dirpath, _, files in os.walk(GAME_AA):
        for f in files:
            if not (f.endswith(".bundle") and f.startswith(BUNDLE_HINT)):
                continue
            for obj in UnityPy.load(os.path.join(dirpath, f)).objects:
                if obj.type.name == "Texture2D" and obj.peek_name() == SPRITE:
                    return obj.read().image.convert("RGBA")
    raise SystemExit(SPRITE + " not found in " + GAME_AA)


def main():
    icon = Image.new("RGBA", (SIZE, SIZE))
    draw = ImageDraw.Draw(icon)
    top, bottom = (58, 52, 70), (22, 20, 30)
    for y in range(SIZE):
        t = y / (SIZE - 1)
        draw.line([(0, y), (SIZE, y)], fill=tuple(int(a + (b - a) * t) for a, b in zip(top, bottom)) + (255,))

    chest = load_sprite()
    chest = chest.crop(chest.getbbox())
    scale = min(150 / chest.width, 112 / chest.height)
    chest = chest.resize((round(chest.width * scale), round(chest.height * scale)), Image.LANCZOS)
    # the sprite is a white mask the game tints; use the legendary chest gold from Colors.md
    tinted = ImageChops.multiply(chest.convert("RGB"), Image.new("RGB", chest.size, (255, 210, 77)))
    chest = Image.merge("RGBA", (*tinted.split(), chest.getchannel("A")))
    icon.alpha_composite(chest, ((SIZE - chest.width) // 2, 14 + (112 - chest.height) // 2))

    font_path = next(p for p in (r"C:\Windows\Fonts\ariblk.ttf", r"C:\Windows\Fonts\arialbd.ttf") if os.path.exists(p))
    draw = ImageDraw.Draw(icon)

    def line(text, y, fill, max_width, start):
        font_size = start
        while True:
            font = ImageFont.truetype(font_path, font_size)
            box = draw.textbbox((0, 0), text, font=font, stroke_width=4)
            if box[2] - box[0] <= max_width:
                break
            font_size -= 1
        x = (SIZE - (box[2] - box[0])) // 2 - box[0]
        draw.text((x, y), text, font=font, fill=fill, stroke_width=4, stroke_fill=(20, 18, 26))
        return draw.textbbox((x, y), text, font=font, stroke_width=4)

    line("MAP", 130, (255, 255, 255), max_width=224, start=56)
    text_bottom = line("CHESTS", 174, (110, 210, 240), max_width=224, start=50)[3]

    target = os.path.join(REPO, "icon.png")
    icon.convert("RGB").save(target)
    print(f"saved {target}, gap below text {SIZE - text_bottom}px")


if __name__ == "__main__":
    main()
