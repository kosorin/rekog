#!/bin/bash

outputDir="cs_volne_knihy"
tempFile="~temp"
source="http://www.volneknihy.xf.cz"
books=(
    "Dante_Alighieri--Bozska_komedie.txt"
    "Jakub_Arbes--Newtonuv_mozek.txt"
    "Jakub_Arbes--Svaty_Xaverius.txt"
    "Bible.txt"
    "Karel_Capek--Anglicke_listy.txt"
    "Karel_Capek--Bila_nemoc.txt"
    "Karel_Capek--Cesta_na_sever.txt"
    "Karel_Capek--Devatero_pohadek.txt"
    "Karel_Capek--Hovory_s_T_G_Masarykem.txt"
    "Karel_Capek--Italske_listy.txt"
    "Karel_Capek--Krakatit.txt"
    "Karel_Capek--O_lidech.txt"
    "Karel_Capek--Obrazky_z_Holandska.txt"
    "Karel_Capek--Povidky_z_druhe_kapsy.txt"
    "Karel_Capek--Povidky_z_jedne_kapsy.txt"
    "Karel_Capek--R_U_R.txt"
    "Karel_Capek--Tovarna_na_Absolutno.txt"
    "Karel_Capek--Valka_s_mloky.txt"
    "Karel_Capek--Vylet_do_Spanel.txt"
    "Karel_Capek--Zahradnikuv_rok.txt"
    "Viktor_Dyk--Krysar.txt"
    "Karel_Jaromir_Erben--Kytice.txt"
    "Karel_Jaromir_Erben--Pohadky.txt"
    "Alois_Jirasek--Stare_povesti_ceske.txt"
    "Jan_Karafiat--Broucci.txt"
    "Moliere--Lakomec.txt"
    "Bozena_Nemcova--Babicka.txt"
    "Jan_Neruda--Povidky_malostranske.txt"
    "Jan_Neruda--Pisne_kosmicke.txt"
    "Blaise_Pascal--Myslenky.txt"
    "Pohadky_tisice_a_jedne_noci.txt"
    "Erich_Maria_Remarque--Na_zapadni_fronte_klid.txt"
    "William_Shakespeare--Hamlet.txt"
    "William_Shakespeare--Romeo_a_Julie.txt"
    "Frantisek_Xaver_Salda--Antonin_Slavicek.txt"
    "Frantisek_Xaver_Salda--Basnicka_osobnost_Dantova.txt"
    "Frantisek_Xaver_Salda--Hodnoty_kulturni_a_mocnosti_zivotni.txt"
    "Frantisek_Xaver_Salda--Impresionismus.txt"
    "Frantisek_Xaver_Salda--Krise_inteligence.txt"
    "Frantisek_Xaver_Salda--Mor_pomnikovy.txt"
    "Frantisek_Xaver_Salda--Nekolik_vzpominek_na_Bohumila_Kubistu.txt"
    "Frantisek_Xaver_Salda--O_tak_zvane_nesmrtelnosti_dila_basnickeho.txt"
    "Frantisek_Xaver_Salda--Stary_a_novy_Manes.txt"
    "Frantisek_Xaver_Salda--Umeni_a_nabozenstvi.txt"
    "Jaroslav_Vrchlicky--Noc_na_Karlstejne.txt"
)
mkdir -p "$outputDir"
for i in "${books[@]}"; do
    echo "Book: $i"
    curl -s "$source/$i" --output "$outputDir/$i"
    iconv -f WINDOWS-1250 -t UTF-8 "$outputDir/$i" -o "$outputDir/$tempFile" && mv "$outputDir/$tempFile" "$outputDir/$i"
done