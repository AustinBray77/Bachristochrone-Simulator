import glob

firstFiles = glob.glob("D:/Solutions/BlockSnake2D/Assets/Scripts/*.cs")
secondFiles = glob.glob("D:/Solutions/BlockSnake2D/Assets/Scripts/*/*.cs")

num_lines = 0

for f in firstFiles:
    num_lines += sum(1 for line in open(f))

for f in secondFiles:
    num_lines += sum(1 for line in open(f))

print(num_lines)