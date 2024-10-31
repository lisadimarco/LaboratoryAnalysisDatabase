-- Database struttura base

-- Tabella Categorie Test
CREATE TABLE TestCategories (
    category_id INT PRIMARY KEY,
    category_name VARCHAR(100),
    description TEXT
);

-- Tabella Test di Laboratorio
CREATE TABLE LabTests (
    test_id INT PRIMARY KEY,
    category_id INT,
    test_name VARCHAR(100),
    abbreviation VARCHAR(20),
    unit VARCHAR(20),
    reference_min DECIMAL(10,2),
    reference_max DECIMAL(10,2),
    gender_specific BOOLEAN,
    age_specific BOOLEAN,
    fasting_required BOOLEAN,
    FOREIGN KEY (category_id) REFERENCES TestCategories(category_id)
);

-- Tabella Range di Riferimento Specifici
CREATE TABLE ReferenceRanges (
    range_id INT PRIMARY KEY,
    test_id INT,
    gender CHAR(1),
    age_min INT,
    age_max INT,
    ref_min DECIMAL(10,2),
    ref_max DECIMAL(10,2),
    FOREIGN KEY (test_id) REFERENCES LabTests(test_id)
);

-- Tabella Pazienti
CREATE TABLE Patients (
    patient_id INT PRIMARY KEY,
    first_name VARCHAR(50),
    last_name VARCHAR(50),
    date_of_birth DATE,
    gender CHAR(1),
    contact_number VARCHAR(15)
);

-- Tabella Risultati Test
CREATE TABLE TestResults (
    result_id INT PRIMARY KEY,
    patient_id INT,
    test_id INT,
    result_value DECIMAL(10,2),
    test_date TIMESTAMP,
    is_abnormal BOOLEAN,
    notes TEXT,
    FOREIGN KEY (patient_id) REFERENCES Patients(patient_id),
    FOREIGN KEY (test_id) REFERENCES LabTests(test_id)
);
