from gistPorter import Porter
import re

class ClearingText(object):
    @classmethod
    def Stemming(self, tokens):
        return [Porter.stem(token) for token in tokens]
    @classmethod
    def ToLowerCase(self, text):
        return text.lower()
    @classmethod
    def DeletePunctuation(self, text):
        text = text.replace("\n", " ")
        text = re.sub(r"[^а-яА-ЯA-Za-z]+", ' ', text)
        text = re.sub(r'[ ]+', " ", text)
        return text
    @classmethod
    def Tokenize(self, text):
        return self.ToLowerCase(text).split(" ")

    @classmethod
    def StartSearching(self, text):
        return self.Stemming(self.Tokenize(self.DeletePunctuation(text)))