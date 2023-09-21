from collections import Counter
from ClearingText import ClearingText

class Helper:
    def __init__(self, id, text):
        self.ID = id
        self.text = text
        self.term_frequencies = None
    
    def __str__(self):
        return f"{self.ID}. {self.text[:50]}"
    
    def analyze(self):
        self.term_frequencies = Counter(ClearingText.StartSearching(self.text))

    def term_frequency(self, term):
        return self.term_frequencies.get(term, 0)
