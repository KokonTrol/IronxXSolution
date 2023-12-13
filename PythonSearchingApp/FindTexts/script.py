from Index import Index
from DBContext import Connection
import sys
# python script.py -DBpath="C:\Users\Kokon\Documents\IronXSolution\ironXsolutionDB" -query="не запускается кс требует обновить стим"
# pyinstaller --onefile script.py
def GetSearchingResult(dictionary, query):
    index = Index()
    for dict in dictionary:
        index.index_document(dict)
    return index.search(query, search_type='OR')

def main():
    try:
        DBpath = sys.argv[1].replace("-DBpath=", "")
        searchQuery = sys.argv[2].replace("-query=", "")
        connection = Connection(DBpath)
        results = [str(res.ID) for res in GetSearchingResult(connection.GetAllHelpers(), searchQuery)]
        if len(results)==0:
            print(" ")
        else:
            print(" ".join(results))
    except:
        print(" ")

if __name__ == "__main__":
    main()

