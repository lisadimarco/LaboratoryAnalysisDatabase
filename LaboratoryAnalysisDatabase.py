import pandas as pd
import sqlite3
from datetime import datetime

class LabDatabase:
    def __init__(self, db_path):
        self.conn = sqlite3.connect(db_path)
        
    def add_test_result(self, patient_id, test_id, value):
        """Aggiunge un risultato test e determina automaticamente se è anomalo"""
        # Ottieni i range di riferimento
        query = """
        SELECT l.reference_min, l.reference_max, l.gender_specific, l.age_specific
        FROM LabTests l
        WHERE l.test_id = ?
        """
        test_info = pd.read_sql(query, self.conn, params=[test_id])
        
        # Se il test è specifico per età/genere, ottieni i range appropriati
        if test_info['age_specific'].iloc[0] or test_info['gender_specific'].iloc[0]:
            # Ottieni info paziente
            patient_query = """
            SELECT gender, date_of_birth
            FROM Patients
            WHERE patient_id = ?
            """
            patient_info = pd.read_sql(patient_query, self.conn, params=[patient_id])
            
            # Calcola età
            age = (datetime.now() - pd.to_datetime(patient_info['date_of_birth'].iloc[0])).years
            
            # Ottieni range specifico
            range_query = """
            SELECT ref_min, ref_max
            FROM ReferenceRanges
            WHERE test_id = ? 
            AND (gender = ? OR gender IS NULL)
            AND age_min <= ? 
            AND age_max >= ?
            """
            ranges = pd.read_sql(range_query, self.conn, 
                               params=[test_id, patient_info['gender'].iloc[0], age, age])
            
            ref_min = ranges['ref_min'].iloc[0]
            ref_max = ranges['ref_max'].iloc[0]
        else:
            ref_min = test_info['reference_min'].iloc[0]
            ref_max = test_info['reference_max'].iloc[0]
        
        # Determina se il valore è anomalo
        is_abnormal = value < ref_min or value > ref_max
        
        # Inserisci il risultato
        cursor = self.conn.cursor()
        cursor.execute("""
        INSERT INTO TestResults (patient_id, test_id, result_value, test_date, is_abnormal)
        VALUES (?, ?, ?, CURRENT_TIMESTAMP, ?)
        """, (patient_id, test_id, value, is_abnormal))
        
        self.conn.commit()
    
    def get_patient_history(self, patient_id):
        """Ottiene la storia dei test del paziente con interpretazione"""
        query = """
        SELECT 
            t.test_date,
            l.test_name,
            t.result_value,
            l.unit,
            t.is_abnormal,
            CASE 
                WHEN t.is_abnormal THEN 
                    CASE 
                        WHEN t.result_value < l.reference_min THEN 'Basso'
                        ELSE 'Alto'
                    END
                ELSE 'Normale'
            END as interpretation
        FROM TestResults t
        JOIN LabTests l ON t.test_id = l.test_id
        WHERE t.patient_id = ?
        ORDER BY t.test_date DESC
        """
        return pd.read_sql(query, self.conn, params=[patient_id])
    
    def analyze_trends(self, test_id):
        """Analizza i trend per un specifico test"""
        query = """
        SELECT 
            DATE(test_date) as date,
            AVG(result_value) as avg_value,
            COUNT(*) as test_count,
            SUM(CASE WHEN is_abnormal THEN 1 ELSE 0 END) as abnormal_count
        FROM TestResults
        WHERE test_id = ?
        GROUP BY DATE(test_date)
        ORDER BY date
        """
        return pd.read_sql(query, self.conn, params=[test_id])

# Esempio di utilizzo
if __name__ == "__main__":
    lab_db = LabDatabase('laboratory.db')
    
    # Aggiungi un risultato
    lab_db.add_test_result(
        patient_id=1,
        test_id=1,  # esempio: emocromo
        value=14.5  # esempio: emoglobina
    )
    
    # Ottieni storia paziente
    history = lab_db.get_patient_history(1)
    print(history)
    
    # Analizza trends
    trends = lab_db.analyze_trends(1)
    print(trends)
