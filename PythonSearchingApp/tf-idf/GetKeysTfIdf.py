from sklearn.feature_extraction.text import TfidfVectorizer
import re, sys, pathlib, os
from pymorphy2 import MorphAnalyzer
from DBContext import Connection
from stopwords import stop_words

if getattr(sys, 'frozen', False) and hasattr(sys, '_MEIPASS'):
    os.environ["PYMORPHY2_DICT_PATH"] = str(pathlib.Path(sys._MEIPASS).joinpath('pymorphy2_dicts_ru/data'))
morph = MorphAnalyzer()
def DeletePunctuation(text):
        text = text.replace("\r\n", " ")
        text = re.sub(r"[^а-яА-ЯA-Za-z]+", ' ', text)
        text = re.sub(r'[ ]+', " ", text)
        return text

def lemmatizeAllText(text):
    tokens = [morph.parse(token)[0].normal_form for token in DeletePunctuation(text).split(" ") if morph.parse(token)[0].normal_form not in stop_words]
    return " ".join(tokens)

def GetKeysTF_IDF(documents, idText):
    vectorizer = TfidfVectorizer()
    texts = [lemmatizeAllText(DeletePunctuation(doc.text)) for doc in documents]
    tfidf_matrix = vectorizer.fit_transform(texts)
    feature_names = vectorizer.get_feature_names_out() 
    for i, doc in enumerate(documents):
        if idText!=doc.ID:
            continue
        tfidf_scores = tfidf_matrix[i]
        sorted_indices = tfidf_scores.toarray()[0].argsort()[::-1]
        top_keywords = [feature_names[idx] for idx in sorted_indices[:10]]
        keywords = top_keywords

    return keywords

def main():
    try:
        DBpath = sys.argv[1].replace("-DBpath=", "")
        idText = int(sys.argv[2].replace("-id=", ""))
        connection = Connection(DBpath)

        results = GetKeysTF_IDF(connection.GetAllHelpers(), idText)
        if len(results)==0:
            print(" ")
        else:
            print(";".join(results))
    except:
        print("Произошла ошибка")

if __name__ == "__main__":
    main()

# pyinstaller GetKeysTfIdf.spec