import os
import pathlib
import sys, re
from stopwords import stop_words
from natasha import (
    Segmenter,
    MorphVocab,
    NewsEmbedding,
    NewsMorphTagger,
    Doc
)
import pymorphy2
if getattr(sys, 'frozen', False) and hasattr(sys, '_MEIPASS'):
    os.environ["PYMORPHY2_DICT_PATH"] = str(pathlib.Path(sys._MEIPASS).joinpath('pymorphy2_dicts_ru/data'))
segmenter = Segmenter()
morph_vocab = MorphVocab()
emb = NewsEmbedding()
morph_tagger = NewsMorphTagger(emb)

morph_analyzer = pymorphy2.MorphAnalyzer()

def DeletePunctuation(text):
        text = text.replace("\r\n", " ")
        text = re.sub(r"[^а-яА-ЯA-Za-z\.]+", ' ', text)
        text = re.sub(r"\s+[а-яА-ЯA-Za-z]{1}\s|^[а-яА-ЯA-Za-z]{1}\s|\s[а-яА-ЯA-Za-z]{1}$", ' ', text)
        text = re.sub(r'[ ]+', " ", text)
        return text

def extract_and_lemmatize_keywords(text):
    lemmatized_keywords = []
    doc = Doc(text)
    doc.segment(segmenter)
    doc.tag_morph(morph_tagger)
    
    for token in doc.tokens:
        if token.pos == 'NOUN' or token.pos == 'ADJ':
            lemma = morph_analyzer.parse(token.text.lower())[0].normal_form
            if lemma not in lemmatized_keywords and lemma not in stop_words:
                lemmatized_keywords.append(lemma)
    
    return lemmatized_keywords

def main():
    try:
        text = DeletePunctuation(sys.argv[1])
        results = extract_and_lemmatize_keywords(text)
        if len(results)==0:
            print(" ")
        else:
            print(";".join(results))
    except:
        print(" ")

if __name__ == "__main__":
    main()
# pyinstaller findkeys.spec