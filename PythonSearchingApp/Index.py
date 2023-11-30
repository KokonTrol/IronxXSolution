from ClearingText import ClearingText
from Helper import Helper

class Index:
    def __init__(self):
        self.index = {}
        self.helpers = {}

    def index_document(self, helper):
        if helper.ID not in self.helpers:
            self.helpers[helper.ID] = helper
            helper.analyze()

        for token in ClearingText.StartSearching(helper.text):
            if token not in self.index:
                self.index[token] = set()
            self.index[token].add(helper.ID)
    
    def results(self, analyzed_query):
        return [self.index.get(token, set()) for token in analyzed_query]

    def search(self, query, search_type='AND', rank=True):
        if search_type not in ('AND', 'OR'):
            return []

        analyzed_query = ClearingText.StartSearching(query)
        results = self.results(analyzed_query)
        if search_type == 'AND':
            helpers = [self.helpers[doc_id] for doc_id in set.intersection(*results)]
        if search_type == 'OR':
            helpers = [self.helpers[doc_id] for doc_id in set.union(*results)]

        if rank:
            return [res[0] for res in self.rank(analyzed_query, helpers)]
        return helpers

    def rank(self, analyzed_query, helpers):
        results = []
        if not helpers:
            return results
        for document in helpers:
            score = sum([document.term_frequency(token) for token in analyzed_query])
            results.append((document, score))
        return sorted(results, key=lambda doc: doc[1], reverse=True)