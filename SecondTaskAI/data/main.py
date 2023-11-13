import networkx as nx

G = nx.Graph()
counte = 0
for i in open('allvkid.txt'):
    counte += 1
    node = list(map(int, i.split(',')))
    G.add_edge(node[0], node[1])

print("Центральность по степени")

characters = sorted(list(nx.degree_centrality(G).items()), key=lambda i: i[1], reverse=True)
print(*characters[:10], sep='\n')

print("Центральность по посредничеству")
characters = sorted(list(nx.betweenness_centrality(G).items()), key=lambda i: i[1], reverse=True)
print(*characters[:10], sep='\n')

print("Центральность по близости")
characters = sorted(list(nx.closeness_centrality(G).items()), key=lambda i: i[1], reverse=True)
print(*characters[:10], sep='\n')