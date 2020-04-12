import codecs

with codecs.open("Credits.txt", "r", "utf-8") as r:
    lines = r.readlines()
    new_lines = []
    can_read = False
    for line in lines:
        if "Package" in line:
            if not line in new_lines:
                can_read = True
                new_lines.append(
                    "####################################################################################################\r\n")
        if can_read:
            if not ("#") in line:
                new_lines.append(line)
            else:
                can_read = False

    with codecs.open("Cleaned Credits.txt", "w+", "utf-8") as f:
        for line in new_lines:
            f.write(line)
