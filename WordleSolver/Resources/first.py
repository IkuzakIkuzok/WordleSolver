
def hamming(w1, w2):
  d = 0
  for c1, c2 in zip(w1, w2):
    d += 0 if c1 == c2 else 1
  return d

letters = 'abcdefghijklmnopqrstuvwxyz'

counts = [
  {l: 0 for l in letters} for _ in range(5)
]

words = []
for l in letters:
  with open(f'./{l}.dat') as dat:
    for w in dat:
      w = w.rstrip()
      words.append(w)
      for d, c in zip(counts, w):
        d[c] += 1

word = ''.join(max(d, key=d.get) for d in counts)
ws = []
md = 5
for w in words:
  d = hamming(word, w)
  if d < md:
    md = d
    ws = [w]
  elif d == md:
    ws.append(w)

print(md)
print(ws)
