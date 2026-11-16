# Ribblin

**Ribblin** je 3D platformer u Unityju u kojem mala žabica jezikom prelazi provalije, jede neprijatelje i skuplja voće kako bi kroz četiri svijeta stigla do četiri bossa.

Projekt iz kolegija *Dizajn i programiranje računalnih igara* (Fakultet informatike u Puli).
Autorica: Lidia Butković (samostalni projekt).

## O igri

- **4 razine**, svaka sa svojom temom, glazbom i bossom:

  | Razina | Boss |
  |---|---|
  | 1. Tropical Island | Captain Pinchbeard |
  | 2. Murky Swamp | Venomaw, the Swamp Serpent |
  | 3. Volcano | Magmor the Molten |
  | 4. Sky Kingdom | Tempestra, Queen of Storms |
- **Jezik** služi za hvatanje za rotkvice iznad provalija i za jedenje neprijatelja.
- **Voće** otključava put do bossa. Zahtjev raste od razine do razine: 14/21, 28/35, 25/29 i 32/38.
- **Tri srca**: kad se izgube sva tri, slijedi **GAME OVER** i razina kreće ispočetka.
- **Portal-vrtlog** na kraju svake razine usiše žabicu u sljedeći svijet.
- **Četiri vrste neprijatelja** (sluz, letač, pljuvač i vatreni zmaj u vulkanu) i **sedam vrsta zamki** (šiljci, njihala, platforme koje padaju, vrteće grede, gejziri, trnje i ptičji izmet koji pada s neba na 4. razini).
- More s **valovima** na prvoj razini, a zmija Venomaw udarcem repa šalje **otrovne valove** koje treba preskočiti.
- **Izbornik**: postavke glasnoće i osjetljivosti miša, izbor razine i najbolje vrijeme. Na kraju je završni ekran sa statistikom.
- Cijela igra traje oko 20 minuta.

## Kontrole

| Radnja | Tipka |
|---|---|
| Kretanje | W A S D / strelice |
| Skok, dvostruki skok | Razmaknica |
| Trčanje (inače hoda) | Lijevi Shift |
| Kamera / zum | Miš / kotačić |
| Jezik | Lijevi klik |
| Pauza i postavke | Esc |

## Tehnologija

- Unity 6000.4.6f1, Universal Render Pipeline (URP), novi Input System
- C# (skripte u `Assets/Scripts`)
- Razine se grade iz koda: u Unityju izbornik **Ribblin → Izgradi cijelu igru** (skripte `Assets/Scripts/Editor/RibblinKit.cs` i `RibblinLevelBuilder.cs`)

## Pokretanje projekta

1. Otvori mapu projekta u Unity Hubu (Unity 6000.4.6f1).
2. U izborniku odaberi **Ribblin → Izgradi cijelu igru**.
3. Otvori scenu `Assets/Levels/Ribblin_Menu` i pritisni Play.

U editoru rade i tipke za testiranje: F1 vodi do vrata bossa, F4 u arenu, F2 udara bossa, F3 preskače razinu, F5 vodi na sljedeći checkpoint, a F6 uključuje način bez štete. U gotovoj igri ne postoje.

## Struktura

```
Assets/
  Levels/        scene (izbornik, 4 razine, kraj)
  Scripts/       skripte igre
    Editor/      alat koji gradi razine
  Resources/
    Music/       glazba + credits.txt
    Sfx/         zvučni efekti
Docs/            dokumentacija (dokument dizajna igre, koncept, dizajn razina)
```

## Licence i zasluge

Popis glazbe i zvučnih efekata s licencama nalazi se u datoteci [CREDITS.md](CREDITS.md).

## Napomena o korištenju generativne umjetne inteligencije

U skladu s pravilima kolegija, alati generativne umjetne inteligencije korišteni su kao pomoć pri pisanju C# koda. Glazba i zvučni efekti nisu generirani umjetnom inteligencijom: to su snimke stvarnih autora pod licencama CC0 i CC BY 4.0.
