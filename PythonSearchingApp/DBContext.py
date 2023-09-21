# import os
# path = os.path.expanduser("~/Documents")
# print(path)


from sqlalchemy import create_engine
from sqlalchemy.orm import DeclarativeBase
from sqlalchemy.orm import Session
from sqlalchemy import  Column, Integer, String
from Helper import Helper

class Base(DeclarativeBase): pass
class HelperInfo(Base):
    __tablename__ = "HelperInfo"

    id = Column(Integer, primary_key=True, index=True)
    HelperInfoText = Column(String)
    Images = Column(String)
    Keys = Column(String)
    Title = Column(String)
    Type = Column(String)

class Connection:
    def __init__(self, pathToDB):
        self.engine = create_engine("sqlite:///"+pathToDB, echo=False)
        Base.metadata.create_all(bind=self.engine)

    def GetAllHelpers(self):
        data = None
        with Session(autoflush=False, bind=self.engine) as db:
            data = db.query(HelperInfo).all()

        helpers = []
        for helper in data:
            helpers.append(Helper(id=helper.id, \
                                text=f"{helper.Title} {helper.Keys} {helper.HelperInfoText}"))
        return helpers