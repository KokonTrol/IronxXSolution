from gistPorter import Porter
import re
from stopwords import stop_words

class ClearingText(object):
    @classmethod
    def Stemming(self, tokens):
        return [Porter.stem(token) for token in tokens]
    @classmethod
    def ToLowerCase(self, text):
        return text.lower()
    @classmethod
    def DeletePunctuation(self, text):
        text = text.replace("\r\n", " ")
        text = re.sub(r"[^а-яА-ЯA-Za-z]+", ' ', text)
        text = re.sub(r'[ ]+', " ", text)
        return text
    @classmethod
    def Tokenize(self, text):
        return [token for token in self.ToLowerCase(text).split(" ") if token not in stop_words]

    @classmethod
    def StartSearching(self, text):
        return self.Stemming(self.Tokenize(self.DeletePunctuation(text)))