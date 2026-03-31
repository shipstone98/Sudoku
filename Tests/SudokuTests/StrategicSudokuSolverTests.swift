//
//  StrategicSudokuSolverTests.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

import Testing
import Utilities

@testable
import Sudoku

@Test
fileprivate func testStrategicSudokuSolver_initializer() {
    // Arrange
    let array = randomArray()
    var sudoku = MockSudoku()
    sudoku.subscriptClosure = { array[$0 * 9 + $1] }
    
    // Act
    let result = StrategicSudokuSolver(sudoku)
    
    // Assert
    #expect(result.moves.isEmpty)
    expectEqual(sudoku, result.sudoku)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_notSolvable() {
    // Arrange
    var sudoku = MockSudoku()
    sudoku.subscriptClosure = { _, _ in 0 }
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    for strategy in SudokuSolverStrategy.allCases {
        // Act
        let result = solver.solve(for: strategy, using: &generator)
        
        // Assert
        #expect(result == nil)
        #expect(solver.moves.isEmpty)
        expectEmpty(solver.sudoku)
    }
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_fullHouse_block() {
    // Arrange
    let string = "800739006370465000040182009000600040054300610060500000400853070000271064100940002"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .fullHouse, using: &generator)
    
    // Assert
    let location = result!.locations.single!
    #expect(location.addedValue == 6)
    #expect(location.column == 5)
    #expect(location.removedCandidates.isEmpty)
    #expect(location.row == 8)
    #expect(result!.strategy == .fullHouse)
    #expect(solver.moves.count == 1)
    
    expectEqual(
        "800739006370465000040182009000600040054300610060500000400853070000271064100946002",
        solver.sudoku
    )
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_fullHouse_column() {
    // Arrange
    let string = "200060000083090000700821900006073000090682040000450100008935004000000290000040008"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .fullHouse, using: &generator)
    
    // Assert
    let location = result!.locations.single!
    #expect(location.addedValue == 1)
    #expect(location.column == 4)
    #expect(location.removedCandidates.isEmpty)
    #expect(location.row == 7)
    #expect(result!.strategy == .fullHouse)
    #expect(solver.moves.count == 1)
    
    expectEqual(
        "200060000083090000700821900006073000090682040000450100008935004000010290000040008",
        solver.sudoku
    )
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_fullHouse_row() {
    // Arrange
    let string = "207000000080090000030600800008164900692785304001320500009001020000040090000000408"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .fullHouse, using: &generator)
    
    // Assert
    let location = result!.locations.single!
    #expect(location.addedValue == 1)
    #expect(location.column == 7)
    #expect(location.removedCandidates.isEmpty)
    #expect(location.row == 4)
    #expect(result!.strategy == .fullHouse)
    #expect(solver.moves.count == 1)
    
    expectEqual(
        "207000000080090000030600800008164900692785314001320500009001020000040090000000408",
        solver.sudoku
    )
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_nakedSingle() {
    // Arrange
    let string = "412736589000000106568010370000850210100000008087090000030070865800000000000908401"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .nakedSingle, using: &generator)
    
    // Assert
    let location = result!.locations.single!
    #expect(location.addedValue == 6)
    #expect(location.column == 6)
    #expect(location.removedCandidates.isEmpty)
    #expect(location.row == 5)
    #expect(result!.strategy == .nakedSingle)
    #expect(solver.moves.count == 1)
    
    expectEqual(
        "412736589000000106568010370000850210100000008087090600030070865800000000000908401",
        solver.sudoku
    )
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_hiddenSingle_block() {
    // Arrange
    let string = "002193000000007000700040019803000600005000230007000504370080006000600000000534100"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .hiddenSingle, using: &generator)
    
    // Assert
    let location = result!.locations.single!
    #expect(location.addedValue == 3)
    #expect(location.column == 3)
    #expect(location.removedCandidates.isEmpty)
    #expect(location.row == 5)
    #expect(result!.strategy == .hiddenSingle)
    #expect(solver.moves.count == 1)
    
    expectEqual(
        "002193000000007000700040019803000600005000230007300504370080006000600000000534100",
        solver.sudoku
    )
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_hiddenSingle_column() {
    // Arrange
    let string = "000100200210300900860700000000270083082934760730006000008003017075000040001007000"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .hiddenSingle, using: &generator)
    
    // Assert
    let location = result!.locations.single!
    #expect(location.addedValue == 6)
    #expect(location.column == 2)
    #expect(location.removedCandidates.isEmpty)
    #expect(location.row == 3)
    #expect(result!.strategy == .hiddenSingle)
    #expect(solver.moves.count == 1)
    
    expectEqual(
        "000100200210300900860700000006270083082934760730006000008003017075000040001007000",
        solver.sudoku
    )
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_hiddenSingle_row() {
    // Arrange
    let string = "028007000016083070000020851137290000000730000000046307290070000000860140000300700"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .hiddenSingle, using: &generator)
    
    // Assert
    let location = result!.locations.single!
    #expect(location.addedValue == 6)
    #expect(location.column == 3)
    #expect(location.removedCandidates.isEmpty)
    #expect(location.row == 2)
    #expect(result!.strategy == .hiddenSingle)
    #expect(solver.moves.count == 1)
    
    expectEqual(
        "028007000016083070000620851137290000000730000000046307290070000000860140000300700",
        solver.sudoku
    )
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_pointingCandidate_column() {
    // Arrange
    let string = "300000000480090000002070000000030069003160002600042083090080007736050098000000005"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .pointingCandidate, using: &generator)
    
    // Assert
    let count = 6
    let column = 6
    let removedCandidate = 1
    let rows = [0, 1, 2, 6, 7, 8]
    #expect(result!.locations.count == count)
    var index = 0
    
    let locations = result!.locations.sorted {
        a, b in
        a.row * 9 + a.column < b.row * 9 + b.column
    }
    
    for location in locations {
        #expect(location.addedValue == nil)
        #expect(location.column == column)
        #expect(location.removedCandidates == [removedCandidate])
        #expect(location.row == rows[index])
        index += 1
    }
    
    #expect(result!.strategy == .pointingCandidate)
    #expect(solver.moves.count == 1)
    expectEqual(string, solver.sudoku)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_pointingCandidate_row() {
    // Arrange
    let string = "340006070080000930002030060000010000097364850000002000000000000000608090000923785"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .pointingCandidate, using: &generator)
    
    // Assert
    let count = 6
    let row = 6
    let removedCandidate = 1
    let columns = [0, 1, 2, 6, 7, 8]
    #expect(result!.locations.count == count)
    var index = 0
    
    let locations = result!.locations.sorted {
        a, b in
        a.row * 9 + a.column < b.row * 9 + b.column
    }
    
    for location in locations {
        #expect(location.addedValue == nil)
        #expect(location.column == columns[index])
        #expect(location.removedCandidates == [removedCandidate])
        #expect(location.row == row)
        index += 1
    }
    
    #expect(result!.strategy == .pointingCandidate)
    #expect(solver.moves.count == 1)
    expectEqual(string, solver.sudoku)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_claimingCandidate_column() {
    // Arrange
    let string = "762008001980000006150000087478003169526009873319800425835001692297685314641932758"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .claimingCandidate, using: &generator)
    
    // Assert
    let count = 6
    let removedCandidate = 4
    let indices = [3, 4, 12, 13, 21, 22]
    #expect(result!.locations.count == count)
    var index = 0
    
    let locations = result!.locations.sorted {
        a, b in
        a.row * 9 + a.column < b.row * 9 + b.column
    }
    
    for location in locations {
        let currentIndex = indices[index]
        #expect(location.addedValue == nil)
        #expect(location.column == currentIndex % 9)
        #expect(location.removedCandidates == [removedCandidate])
        #expect(location.row == currentIndex / 9)
        index += 1
    }
    
    #expect(result!.strategy == .claimingCandidate)
    #expect(solver.moves.count == 1)
    expectEqual(string, solver.sudoku)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_claimingCandidate_row() {
    // Arrange
    let string = "791453826685721394200869571000008069000000083800390152000184637008672915167935248"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .claimingCandidate, using: &generator)
    
    // Assert
    let count = 6
    let removedCandidate = 4
    let indices = [27, 28, 29, 36, 37, 38]
    #expect(result!.locations.count == count)
    var index = 0
    
    let locations = result!.locations.sorted {
        a, b in
        a.row * 9 + a.column < b.row * 9 + b.column
    }
    
    for location in locations {
        let currentIndex = indices[index]
        #expect(location.addedValue == nil)
        #expect(location.column == currentIndex % 9)
        #expect(location.removedCandidates == [removedCandidate])
        #expect(location.row == currentIndex / 9)
        index += 1
    }
    
    #expect(result!.strategy == .claimingCandidate)
    #expect(solver.moves.count == 1)
    expectEqual(string, solver.sudoku)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_nakedPair_block() {
    // Arrange
    let string = "000004000000002000000356000310007246760000305000000000000001000000000000000000000"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .nakedPair, using: &generator)
    
    // Assert
    let count = 6
    let removedCandidates: Set<Int> = [8, 9]
    let indices = [30, 39, 40, 48, 49, 50]
    #expect(result!.locations.count == count)
    var index = 0
    
    let locations = result!.locations.sorted {
        a, b in
        a.row * 9 + a.column < b.row * 9 + b.column
    }
    
    for location in locations {
        let currentIndex = indices[index]
        #expect(location.addedValue == nil)
        #expect(location.column == currentIndex % 9)
        #expect(location.removedCandidates == removedCandidates)
        #expect(location.row == currentIndex / 9)
        index += 1
    }
    
    #expect(result!.strategy == .nakedPair)
    #expect(solver.moves.count == 1)
    expectEqual(string, solver.sudoku)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_nakedPair_column() {
    // Arrange
    let string = "794638215020491000080275400812746500436859100957312684000963000308520961069180300"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .nakedPair, using: &generator)
    
    // Assert
    let location = result!.locations.single!
    #expect(location.addedValue == nil)
    #expect(location.column == 7)
    #expect(location.removedCandidates == [3])
    #expect(location.row == 1)
    #expect(result!.strategy == .nakedPair)
    #expect(solver.moves.count == 1)
    expectEqual(string, solver.sudoku)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_nakedPair_row() {
    // Arrange
    let string = "700849030928135006400267089642783951397451628815692300204516093100008060500004010"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .nakedPair, using: &generator)
    
    // Assert
    let location = result!.locations.single!
    #expect(location.addedValue == nil)
    #expect(location.column == 1)
    #expect(location.removedCandidates == [3])
    #expect(location.row == 7)
    #expect(result!.strategy == .nakedPair)
    #expect(solver.moves.count == 1)
    expectEqual(string, solver.sudoku)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_hiddenPair_block() {
    // Arrange
    let string = "000060000000042736006730040094000068000096407607050923100000085060080271005010090"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .hiddenPair, using: &generator)
    
    // Assert
    #expect(result!.locations.count == 2)
    
    let locations = result!.locations.sorted {
        a, b in
        a.row * 9 + a.column < b.row * 9 + b.column
    }
    
    #expect(locations.first!.addedValue == nil)
    #expect(locations.first!.column == 0)
    #expect(locations.first!.removedCandidates.sorted() == [2, 3, 5, 8, 9])
    #expect(locations.first!.row == 0)
    #expect(locations.last!.addedValue == nil)
    #expect(locations.last!.column == 1)
    #expect(locations.last!.removedCandidates.sorted() == [1, 2, 3, 5, 8])
    #expect(locations.last!.row == 0)
    #expect(result!.strategy == .hiddenPair)
    #expect(solver.moves.count == 1)
    expectEqual(string, solver.sudoku)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_hiddenPair_column() {
    // Arrange
    let string = "000006100000090060006407005007000000643095081020060000070049020034602879060873514"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .hiddenPair, using: &generator)
    
    // Assert
    #expect(result!.locations.count == 2)
    
    let locations = result!.locations.sorted {
        a, b in
        a.row * 9 + a.column < b.row * 9 + b.column
    }
    
    #expect(locations.first!.addedValue == nil)
    #expect(locations.first!.column == 0)
    #expect(locations.first!.removedCandidates.sorted() == [2, 3, 5, 8, 9])
    #expect(locations.first!.row == 0)
    #expect(locations.last!.addedValue == nil)
    #expect(locations.last!.column == 0)
    #expect(locations.last!.removedCandidates.sorted() == [1, 2, 3, 5, 8])
    #expect(locations.last!.row == 1)
    #expect(result!.strategy == .hiddenPair)
    #expect(solver.moves.count == 1)
    expectEqual(string, solver.sudoku)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_hiddenPair_row() {
    // Arrange
    let string = "000060000000042736006730040094000068000096407607050923100000085060080271005010094"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .hiddenPair, using: &generator)
    
    // Assert
    #expect(result!.locations.count == 2)
    
    let locations = result!.locations.sorted {
        a, b in
        a.row * 9 + a.column < b.row * 9 + b.column
    }
    
    #expect(locations.first!.addedValue == nil)
    #expect(locations.first!.column == 0)
    #expect(locations.first!.removedCandidates.sorted() == [2, 3, 5, 8, 9])
    #expect(locations.first!.row == 0)
    #expect(locations.last!.addedValue == nil)
    #expect(locations.last!.column == 1)
    #expect(locations.last!.removedCandidates.sorted() == [1, 2, 3, 5, 8])
    #expect(locations.last!.row == 0)
    #expect(result!.strategy == .hiddenPair)
    #expect(solver.moves.count == 1)
    expectEqual(string, solver.sudoku)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_xWing_column() {
    // Arrange
    let string = "980062753065003000327050006790030500050009000832045009673591428249087005518020007"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .xWing, using: &generator)
    
    // Assert
    let indices = [12, 15, 16, 17, 38, 39, 42, 43, 44]
    #expect(result!.locations.count == indices.count)
    let removedCandidate = 1
    var index = 0
    
    let locations = result!.locations.sorted {
        a, b in
        a.row * 9 + a.column < b.row * 9 + b.column
    }
    
    for location in locations {
        let currentIndex = indices[index]
        #expect(location.addedValue == nil)
        #expect(location.column == currentIndex % 9)
        #expect(location.removedCandidates.single == removedCandidate)
        #expect(location.row == currentIndex / 9)
        index += 1
    }
    
    #expect(result!.strategy == .xWing)
    #expect(solver.moves.count == 1)
    expectEqual(string, solver.sudoku)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_xWing_row() {
    // Arrange
    let string = "903708625862953741057002398000000500605304982230095170700500400500000200306009857"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .xWing, using: &generator)
    
    // Assert
    let indices = [22, 28, 31, 55, 58, 64, 67, 73, 76]
    #expect(result!.locations.count == indices.count)
    let removedCandidate = 1
    var index = 0
    
    let locations = result!.locations.sorted {
        a, b in
        a.row * 9 + a.column < b.row * 9 + b.column
    }
    
    for location in locations {
        let currentIndex = indices[index]
        #expect(location.addedValue == nil)
        #expect(location.column == currentIndex % 9)
        #expect(location.removedCandidates.single == removedCandidate)
        #expect(location.row == currentIndex / 9)
        index += 1
    }
    
    #expect(result!.strategy == .xWing)
    #expect(solver.moves.count == 1)
    expectEqual(string, solver.sudoku)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_bugPlus1() {
    // Arrange
    let string = "140780009280450107370610008953871002724965813861324975618237594597148326432596781"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    
    // Act
    let result = solver.solve(for: .bugPlus1, using: &generator)
    
    // Assert
    let location = result!.locations.single!
    #expect(location.addedValue == 6)
    #expect(location.column == 7)
    #expect(location.removedCandidates.isEmpty)
    #expect(location.row == 0)
    #expect(result!.strategy == .bugPlus1)
    #expect(solver.moves.count == 1)
    
    expectEqual(
        "140780069280450107370610008953871002724965813861324975618237594597148326432596781",
        solver.sudoku
    )
}
