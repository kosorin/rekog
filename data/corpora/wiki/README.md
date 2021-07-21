1. Stáhnou a rozbalit `pages-articles.xml.bz2` z https://dumps.wikimedia.org/<language>wiki/ (jazyk může být např. `cs` nebo `en`)
2. Nainstalovat wiki XML extraktor `pip3 install wikiextractor`
3. Extrahovat data `python3 -m wikiextractor.WikiExtractor <input>`
4. Odebrat XML tagy `sed -i -e 's/^<\/doc>.*//g' -e 's/^<doc id=".*//g' <input_*.txt>`
